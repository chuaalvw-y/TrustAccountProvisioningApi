using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ChuA.DatabaseLegacy;
using ChuA.ObservabilityLegacy.Abstractions;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public class TrustAccountService : ITrustAccountService
    {
        private const string SelectColumns = @"
TrustAccountId,
AccountNumber,
AccountName,
AccountDescription,
AccountStatus,
AccountOpenDate,
AccountCloseDate,
AccountDomicile,
AccountBasisName,
AccountProfitCenter,
OrganizationId,
SubdivisionId,
RelationshipId,
ProductLineId,
CapacityCode,
SimpleComplexTrust,
AccountOfficerId,
AccountOfficerName,
BackupAccountOfficerId,
SeniorAdministrator,
InvestmentOfficerId,
InvestmentOfficerName,
InvestmentObjective,
PortfolioModel,
PortfolioType,
AccrualRequiredFlag,
AccretionMethod,
AccretionMethodModelId,
AmortizationMethod,
TaxId,
TaxIdType,
TaxJurisdiction,
TaxFederalProfile,
FiscalYearEnd,
TaxYearEndMonthNumber,
NonResidentAlienWithholdingFlag,
ExemptForeignWithholdingFlag,
Us1099ReportFlag,
Prepare1099ReturnFlag,
CreatedBy,
CreatedDate,
ModifiedBy,
ModifiedDate";

        private readonly IChuADatabase _database;
        private readonly IApplicationLogger _logger;

        public TrustAccountService(
            IChuADatabase database,
            IApplicationLogger logger)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<TrustAccountResponse> Search(TrustAccountSearchRequest request)
        {
            request = request ?? new TrustAccountSearchRequest();
            _logger.Information(
                "Searching trust accounts for AccountNumber {AccountNumber}, AccountStatus {AccountStatus}, OrganizationId {OrganizationId}, RelationshipId {RelationshipId}, and AccountOfficerId {AccountOfficerId}.",
                AccountNumberMasker.Mask(request.AccountNumber),
                request.AccountStatus,
                request.OrganizationId,
                request.RelationshipId,
                request.AccountOfficerId);

            var sql = $@"
SELECT {SelectColumns}
FROM account.TrustAccount
WHERE (@AccountNumber IS NULL OR AccountNumber LIKE @AccountNumber + '%')
  AND (@AccountStatus IS NULL OR AccountStatus = @AccountStatus)
  AND (@OrganizationId IS NULL OR OrganizationId = @OrganizationId)
  AND (@RelationshipId IS NULL OR RelationshipId = @RelationshipId)
  AND (@AccountOfficerId IS NULL OR AccountOfficerId = @AccountOfficerId)
ORDER BY CreatedDate DESC;";

            var rows = new List<TrustAccountResponse>();

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumber", NullIfWhiteSpace(request.AccountNumber));
                AddParameter(command, "@AccountStatus", NullIfWhiteSpace(request.AccountStatus));
                AddParameter(command, "@OrganizationId", NullIfWhiteSpace(request.OrganizationId));
                AddParameter(command, "@RelationshipId", NullIfWhiteSpace(request.RelationshipId));
                AddParameter(command, "@AccountOfficerId", NullIfWhiteSpace(request.AccountOfficerId));

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(MapTrustAccount(reader));
                    }
                }
            }

            return rows;
        }

        public TrustAccountResponse Get(Guid trustAccountId)
        {
            _logger.Information("Getting trust account {TrustAccountId}.", trustAccountId);

            var sql = $@"
SELECT {SelectColumns}
FROM account.TrustAccount
WHERE TrustAccountId = @TrustAccountId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@TrustAccountId", trustAccountId);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapTrustAccount(reader) : null;
                }
            }
        }

        public TrustAccountResponse Create(TrustAccountCreateRequest request)
        {
            ValidateCreate(request);
            _logger.Information(
                "Creating trust account for AccountNumber {AccountNumber}.",
                AccountNumberMasker.Mask(request.AccountNumber));

            var sql = $@"
INSERT account.TrustAccount
       (AccountNumber, AccountName, AccountDescription, AccountStatus, AccountOpenDate,
        AccountCloseDate, AccountDomicile, AccountBasisName, AccountProfitCenter,
        OrganizationId, SubdivisionId, RelationshipId, ProductLineId, CapacityCode,
        SimpleComplexTrust, AccountOfficerId, AccountOfficerName, BackupAccountOfficerId,
        SeniorAdministrator, InvestmentOfficerId, InvestmentOfficerName, InvestmentObjective,
        PortfolioModel, PortfolioType, AccrualRequiredFlag, AccretionMethod,
        AccretionMethodModelId, AmortizationMethod, TaxId, TaxIdType, TaxJurisdiction,
        TaxFederalProfile, FiscalYearEnd, TaxYearEndMonthNumber,
        NonResidentAlienWithholdingFlag, ExemptForeignWithholdingFlag, Us1099ReportFlag,
        Prepare1099ReturnFlag, CreatedBy)
OUTPUT {SqlColumnList.ToInsertedColumns(SelectColumns)}
VALUES (@AccountNumber, @AccountName, @AccountDescription, @AccountStatus, @AccountOpenDate,
        @AccountCloseDate, @AccountDomicile, @AccountBasisName, @AccountProfitCenter,
        @OrganizationId, @SubdivisionId, @RelationshipId, @ProductLineId, @CapacityCode,
        @SimpleComplexTrust, @AccountOfficerId, @AccountOfficerName, @BackupAccountOfficerId,
        @SeniorAdministrator, @InvestmentOfficerId, @InvestmentOfficerName, @InvestmentObjective,
        @PortfolioModel, @PortfolioType, @AccrualRequiredFlag, @AccretionMethod,
        @AccretionMethodModelId, @AmortizationMethod, @TaxId, @TaxIdType, @TaxJurisdiction,
        @TaxFederalProfile, @FiscalYearEnd, @TaxYearEndMonthNumber,
        @NonResidentAlienWithholdingFlag, @ExemptForeignWithholdingFlag, @Us1099ReportFlag,
        @Prepare1099ReturnFlag, @CreatedBy);";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddRequestParameters(command, request);
                AddParameter(command, "@CreatedBy", request.CreatedBy);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapTrustAccount(reader);
                    }
                }
            }

            throw new InvalidOperationException("Trust account was not created.");
        }

        public TrustAccountResponse Update(TrustAccountUpdateRequest request)
        {
            ValidateUpdate(request);
            _logger.Information("Updating trust account {TrustAccountId}.", request.TrustAccountId);

            var sql = $@"
UPDATE account.TrustAccount
SET AccountNumber = @AccountNumber,
    AccountName = @AccountName,
    AccountDescription = @AccountDescription,
    AccountStatus = @AccountStatus,
    AccountOpenDate = @AccountOpenDate,
    AccountCloseDate = @AccountCloseDate,
    AccountDomicile = @AccountDomicile,
    AccountBasisName = @AccountBasisName,
    AccountProfitCenter = @AccountProfitCenter,
    OrganizationId = @OrganizationId,
    SubdivisionId = @SubdivisionId,
    RelationshipId = @RelationshipId,
    ProductLineId = @ProductLineId,
    CapacityCode = @CapacityCode,
    SimpleComplexTrust = @SimpleComplexTrust,
    AccountOfficerId = @AccountOfficerId,
    AccountOfficerName = @AccountOfficerName,
    BackupAccountOfficerId = @BackupAccountOfficerId,
    SeniorAdministrator = @SeniorAdministrator,
    InvestmentOfficerId = @InvestmentOfficerId,
    InvestmentOfficerName = @InvestmentOfficerName,
    InvestmentObjective = @InvestmentObjective,
    PortfolioModel = @PortfolioModel,
    PortfolioType = @PortfolioType,
    AccrualRequiredFlag = @AccrualRequiredFlag,
    AccretionMethod = @AccretionMethod,
    AccretionMethodModelId = @AccretionMethodModelId,
    AmortizationMethod = @AmortizationMethod,
    TaxId = @TaxId,
    TaxIdType = @TaxIdType,
    TaxJurisdiction = @TaxJurisdiction,
    TaxFederalProfile = @TaxFederalProfile,
    FiscalYearEnd = @FiscalYearEnd,
    TaxYearEndMonthNumber = @TaxYearEndMonthNumber,
    NonResidentAlienWithholdingFlag = @NonResidentAlienWithholdingFlag,
    ExemptForeignWithholdingFlag = @ExemptForeignWithholdingFlag,
    Us1099ReportFlag = @Us1099ReportFlag,
    Prepare1099ReturnFlag = @Prepare1099ReturnFlag,
    ModifiedBy = @ModifiedBy,
    ModifiedDate = SYSUTCDATETIME()
OUTPUT {SqlColumnList.ToInsertedColumns(SelectColumns)}
WHERE TrustAccountId = @TrustAccountId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@TrustAccountId", request.TrustAccountId);
                AddRequestParameters(command, request);
                AddParameter(command, "@ModifiedBy", request.ModifiedBy);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapTrustAccount(reader) : null;
                }
            }
        }

        public bool Delete(Guid trustAccountId)
        {
            _logger.Information("Deleting trust account {TrustAccountId}.", trustAccountId);

            const string sql = @"
DELETE account.TrustAccount
WHERE TrustAccountId = @TrustAccountId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@TrustAccountId", trustAccountId);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public async Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest request)
        {
            ValidateCreateAccount(request);

            _logger.Information(
                "Stored procedure account creation started for AccountNumber {AccountNumber}.",
                AccountNumberMasker.Mask(request.AccountNumber));

            using (var connection = CreateSqlConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "account.CreateAccount";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(CreateSqlParameter(
                    "@CreatedBy",
                    SqlDbType.VarChar,
                    50,
                    request.CreatedBy));
                command.Parameters.Add(CreateSqlParameter(
                    "@AccountNumber",
                    SqlDbType.VarChar,
                    16,
                    NullIfWhiteSpace(request.AccountNumber)));

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        _logger.Warning(
                            "Stored procedure account creation failed because no result row was returned.");

                        return new CreateAccountResponse
                        {
                            Success = false,
                            Message = "Account creation did not return a result."
                        };
                    }

                    var response = MapCreateAccountResponse(reader);

                    if (response.Success)
                    {
                        _logger.Information(
                            "Stored procedure account creation succeeded for AccountNumber {AccountNumber}.",
                            AccountNumberMasker.Mask(response.AccountNumber));
                    }
                    else
                    {
                        _logger.Warning(
                            "Stored procedure account creation failed with message {Message}.",
                            response.Message);
                    }

                    return response;
                }
            }
        }

        private IDbConnection CreateConnection()
        {
            return CreateSqlConnection();
        }

        private SqlConnection CreateSqlConnection()
        {
            var connectionString = _database.Options?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "The ChuA.DatabaseLegacy connection string is not configured.");
            }

            return new SqlConnection(connectionString);
        }

        private static SqlParameter CreateSqlParameter(
            string name,
            SqlDbType type,
            int size,
            object value)
        {
            return new SqlParameter(name, type, size)
            {
                Value = value ?? DBNull.Value
            };
        }

        private static void AddRequestParameters(IDbCommand command, TrustAccountCreateRequest request)
        {
            AddParameter(command, "@AccountNumber", request.AccountNumber);
            AddParameter(command, "@AccountName", request.AccountName);
            AddParameter(command, "@AccountDescription", NullIfWhiteSpace(request.AccountDescription));
            AddParameter(command, "@AccountStatus", request.AccountStatus);
            AddParameter(command, "@AccountOpenDate", request.AccountOpenDate);
            AddParameter(command, "@AccountCloseDate", request.AccountCloseDate);
            AddParameter(command, "@AccountDomicile", NullIfWhiteSpace(request.AccountDomicile));
            AddParameter(command, "@AccountBasisName", NullIfWhiteSpace(request.AccountBasisName));
            AddParameter(command, "@AccountProfitCenter", NullIfWhiteSpace(request.AccountProfitCenter));
            AddParameter(command, "@OrganizationId", NullIfWhiteSpace(request.OrganizationId));
            AddParameter(command, "@SubdivisionId", NullIfWhiteSpace(request.SubdivisionId));
            AddParameter(command, "@RelationshipId", NullIfWhiteSpace(request.RelationshipId));
            AddParameter(command, "@ProductLineId", NullIfWhiteSpace(request.ProductLineId));
            AddParameter(command, "@CapacityCode", NullIfWhiteSpace(request.CapacityCode));
            AddParameter(command, "@SimpleComplexTrust", NullIfWhiteSpace(request.SimpleComplexTrust));
            AddParameter(command, "@AccountOfficerId", NullIfWhiteSpace(request.AccountOfficerId));
            AddParameter(command, "@AccountOfficerName", NullIfWhiteSpace(request.AccountOfficerName));
            AddParameter(command, "@BackupAccountOfficerId", NullIfWhiteSpace(request.BackupAccountOfficerId));
            AddParameter(command, "@SeniorAdministrator", NullIfWhiteSpace(request.SeniorAdministrator));
            AddParameter(command, "@InvestmentOfficerId", NullIfWhiteSpace(request.InvestmentOfficerId));
            AddParameter(command, "@InvestmentOfficerName", NullIfWhiteSpace(request.InvestmentOfficerName));
            AddParameter(command, "@InvestmentObjective", NullIfWhiteSpace(request.InvestmentObjective));
            AddParameter(command, "@PortfolioModel", NullIfWhiteSpace(request.PortfolioModel));
            AddParameter(command, "@PortfolioType", NullIfWhiteSpace(request.PortfolioType));
            AddParameter(command, "@AccrualRequiredFlag", request.AccrualRequiredFlag);
            AddParameter(command, "@AccretionMethod", NullIfWhiteSpace(request.AccretionMethod));
            AddParameter(command, "@AccretionMethodModelId", NullIfWhiteSpace(request.AccretionMethodModelId));
            AddParameter(command, "@AmortizationMethod", NullIfWhiteSpace(request.AmortizationMethod));
            AddParameter(command, "@TaxId", NullIfWhiteSpace(request.TaxId));
            AddParameter(command, "@TaxIdType", NullIfWhiteSpace(request.TaxIdType));
            AddParameter(command, "@TaxJurisdiction", NullIfWhiteSpace(request.TaxJurisdiction));
            AddParameter(command, "@TaxFederalProfile", NullIfWhiteSpace(request.TaxFederalProfile));
            AddParameter(command, "@FiscalYearEnd", NullIfWhiteSpace(request.FiscalYearEnd));
            AddParameter(command, "@TaxYearEndMonthNumber", NullIfWhiteSpace(request.TaxYearEndMonthNumber));
            AddParameter(command, "@NonResidentAlienWithholdingFlag", request.NonResidentAlienWithholdingFlag);
            AddParameter(command, "@ExemptForeignWithholdingFlag", request.ExemptForeignWithholdingFlag);
            AddParameter(command, "@Us1099ReportFlag", request.Us1099ReportFlag);
            AddParameter(command, "@Prepare1099ReturnFlag", request.Prepare1099ReturnFlag);
        }

        private static TrustAccountResponse MapTrustAccount(IDataRecord record)
        {
            return new TrustAccountResponse
            {
                TrustAccountId = record.GetGuid(record.GetOrdinal("TrustAccountId")),
                AccountNumber = record.GetString(record.GetOrdinal("AccountNumber")),
                AccountName = record.GetString(record.GetOrdinal("AccountName")),
                AccountDescription = GetNullableString(record, "AccountDescription"),
                AccountStatus = record.GetString(record.GetOrdinal("AccountStatus")),
                AccountOpenDate = GetNullableDateTime(record, "AccountOpenDate"),
                AccountCloseDate = GetNullableDateTime(record, "AccountCloseDate"),
                AccountDomicile = GetNullableString(record, "AccountDomicile"),
                AccountBasisName = GetNullableString(record, "AccountBasisName"),
                AccountProfitCenter = GetNullableString(record, "AccountProfitCenter"),
                OrganizationId = GetNullableString(record, "OrganizationId"),
                SubdivisionId = GetNullableString(record, "SubdivisionId"),
                RelationshipId = GetNullableString(record, "RelationshipId"),
                ProductLineId = GetNullableString(record, "ProductLineId"),
                CapacityCode = GetNullableString(record, "CapacityCode"),
                SimpleComplexTrust = GetNullableString(record, "SimpleComplexTrust"),
                AccountOfficerId = GetNullableString(record, "AccountOfficerId"),
                AccountOfficerName = GetNullableString(record, "AccountOfficerName"),
                BackupAccountOfficerId = GetNullableString(record, "BackupAccountOfficerId"),
                SeniorAdministrator = GetNullableString(record, "SeniorAdministrator"),
                InvestmentOfficerId = GetNullableString(record, "InvestmentOfficerId"),
                InvestmentOfficerName = GetNullableString(record, "InvestmentOfficerName"),
                InvestmentObjective = GetNullableString(record, "InvestmentObjective"),
                PortfolioModel = GetNullableString(record, "PortfolioModel"),
                PortfolioType = GetNullableString(record, "PortfolioType"),
                AccrualRequiredFlag = record.GetBoolean(record.GetOrdinal("AccrualRequiredFlag")),
                AccretionMethod = GetNullableString(record, "AccretionMethod"),
                AccretionMethodModelId = GetNullableString(record, "AccretionMethodModelId"),
                AmortizationMethod = GetNullableString(record, "AmortizationMethod"),
                MaskedTaxId = SensitiveValueMasker.MaskLastFour(GetNullableString(record, "TaxId")),
                TaxIdType = GetNullableString(record, "TaxIdType"),
                TaxJurisdiction = GetNullableString(record, "TaxJurisdiction"),
                TaxFederalProfile = GetNullableString(record, "TaxFederalProfile"),
                FiscalYearEnd = GetNullableString(record, "FiscalYearEnd"),
                TaxYearEndMonthNumber = GetNullableString(record, "TaxYearEndMonthNumber"),
                NonResidentAlienWithholdingFlag = record.GetBoolean(record.GetOrdinal("NonResidentAlienWithholdingFlag")),
                ExemptForeignWithholdingFlag = record.GetBoolean(record.GetOrdinal("ExemptForeignWithholdingFlag")),
                Us1099ReportFlag = record.GetBoolean(record.GetOrdinal("Us1099ReportFlag")),
                Prepare1099ReturnFlag = record.GetBoolean(record.GetOrdinal("Prepare1099ReturnFlag")),
                CreatedBy = record.GetString(record.GetOrdinal("CreatedBy")),
                CreatedDate = record.GetDateTime(record.GetOrdinal("CreatedDate")),
                ModifiedBy = GetNullableString(record, "ModifiedBy"),
                ModifiedDate = GetNullableDateTime(record, "ModifiedDate")
            };
        }

        private static CreateAccountResponse MapCreateAccountResponse(IDataRecord record)
        {
            return new CreateAccountResponse
            {
                Success = record.GetBoolean(record.GetOrdinal("Success")),
                AccountNumber = GetNullableString(record, "AccountNumber"),
                Message = GetNullableString(record, "Message")
            };
        }

        private static void ValidateCreateAccount(CreateAccountRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            RequireString(request.CreatedBy, nameof(request.CreatedBy), 50);
            OptionalString(request.AccountNumber, nameof(request.AccountNumber), 16);
        }

        private static void ValidateCreate(TrustAccountCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateRequest(request);
            RequireString(request.CreatedBy, nameof(request.CreatedBy), 100);
        }

        private static void ValidateUpdate(TrustAccountUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.TrustAccountId == Guid.Empty)
            {
                throw new ArgumentException("TrustAccountId is required.");
            }

            ValidateRequest(request);
            RequireString(request.ModifiedBy, nameof(request.ModifiedBy), 100);
        }

        private static void ValidateRequest(TrustAccountCreateRequest request)
        {
            RequireString(request.AccountNumber, nameof(request.AccountNumber), 16);
            RequireString(request.AccountName, nameof(request.AccountName), 100);
            RequireString(request.AccountStatus, nameof(request.AccountStatus), 30);
            OptionalString(request.AccountDescription, nameof(request.AccountDescription), 254);
            OptionalString(request.AccountDomicile, nameof(request.AccountDomicile), 8);
            OptionalString(request.AccountBasisName, nameof(request.AccountBasisName), 20);
            OptionalString(request.AccountProfitCenter, nameof(request.AccountProfitCenter), 16);
            OptionalString(request.OrganizationId, nameof(request.OrganizationId), 16);
            OptionalString(request.SubdivisionId, nameof(request.SubdivisionId), 16);
            OptionalString(request.RelationshipId, nameof(request.RelationshipId), 16);
            OptionalString(request.ProductLineId, nameof(request.ProductLineId), 8);
            OptionalString(request.CapacityCode, nameof(request.CapacityCode), 3);
            OptionalString(request.SimpleComplexTrust, nameof(request.SimpleComplexTrust), 8);
            OptionalString(request.AccountOfficerId, nameof(request.AccountOfficerId), 16);
            OptionalString(request.AccountOfficerName, nameof(request.AccountOfficerName), 100);
            OptionalString(request.BackupAccountOfficerId, nameof(request.BackupAccountOfficerId), 16);
            OptionalString(request.SeniorAdministrator, nameof(request.SeniorAdministrator), 16);
            OptionalString(request.InvestmentOfficerId, nameof(request.InvestmentOfficerId), 16);
            OptionalString(request.InvestmentOfficerName, nameof(request.InvestmentOfficerName), 100);
            OptionalString(request.InvestmentObjective, nameof(request.InvestmentObjective), 8);
            OptionalString(request.PortfolioModel, nameof(request.PortfolioModel), 12);
            OptionalString(request.PortfolioType, nameof(request.PortfolioType), 1);
            OptionalString(request.AccretionMethod, nameof(request.AccretionMethod), 8);
            OptionalString(request.AccretionMethodModelId, nameof(request.AccretionMethodModelId), 16);
            OptionalString(request.AmortizationMethod, nameof(request.AmortizationMethod), 8);
            OptionalString(request.TaxId, nameof(request.TaxId), 9);
            OptionalString(request.TaxIdType, nameof(request.TaxIdType), 8);
            OptionalString(request.TaxJurisdiction, nameof(request.TaxJurisdiction), 8);
            OptionalString(request.TaxFederalProfile, nameof(request.TaxFederalProfile), 8);
            OptionalString(request.FiscalYearEnd, nameof(request.FiscalYearEnd), 5);
            OptionalString(request.TaxYearEndMonthNumber, nameof(request.TaxYearEndMonthNumber), 2);
        }

        private static void AddParameter(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static string GetNullableString(IDataRecord record, string name)
        {
            var ordinal = record.GetOrdinal(name);
            return record.IsDBNull(ordinal) ? null : record.GetString(ordinal);
        }

        private static DateTime? GetNullableDateTime(IDataRecord record, string name)
        {
            var ordinal = record.GetOrdinal(name);
            return record.IsDBNull(ordinal) ? (DateTime?)null : record.GetDateTime(ordinal);
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static void RequireString(string value, string name, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{name} is required.");
            }

            OptionalString(value, name, maxLength);
        }

        private static void OptionalString(string value, string name, int maxLength)
        {
            if (value != null && value.Length > maxLength)
            {
                throw new ArgumentException($"{name} cannot exceed {maxLength} characters.");
            }
        }
    }
}
