using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChuA.DatabaseLegacy;
using ChuA.ObservabilityLegacy.Abstractions;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public class AccountNumberListService : IAccountNumberListService
    {
        private readonly IChuADatabase _database;
        private readonly IApplicationLogger _logger;

        public AccountNumberListService(
            IChuADatabase database,
            IApplicationLogger logger)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<AccountNumberListResponse> Search(AccountNumberListSearchRequest request)
        {
            request = request ?? new AccountNumberListSearchRequest();
            _logger.Information(
                "Searching account number list entries for AccountNumber {AccountNumber} and EmployeeId {EmployeeId}.",
                AccountNumberMasker.Mask(request.AccountNumber),
                request.EmployeeId);

            const string sql = @"
SELECT AccountNumberListId,
       AccountNumber,
       EmployeeId,
       Manual,
       Notes,
       CreatedDate,
       CreatedBy,
       ModifiedDate,
       ModifiedBy
FROM account.AccountNumberList
WHERE (@AccountNumber IS NULL OR AccountNumber = @AccountNumber)
  AND (@EmployeeId IS NULL OR EmployeeId = @EmployeeId)
ORDER BY CreatedDate DESC;";

            var rows = new List<AccountNumberListResponse>();

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumber", NullIfWhiteSpace(request.AccountNumber));
                AddParameter(command, "@EmployeeId", NullIfWhiteSpace(request.EmployeeId));

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(MapAccountNumberList(reader));
                    }
                }
            }

            return rows;
        }

        public AccountNumberListResponse Get(Guid accountNumberListId)
        {
            _logger.Information(
                "Getting account number list entry {AccountNumberListId}.",
                accountNumberListId);

            const string sql = @"
SELECT AccountNumberListId,
       AccountNumber,
       EmployeeId,
       Manual,
       Notes,
       CreatedDate,
       CreatedBy,
       ModifiedDate,
       ModifiedBy
FROM account.AccountNumberList
WHERE AccountNumberListId = @AccountNumberListId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumberListId", accountNumberListId);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read()
                        ? MapAccountNumberList(reader)
                        : null;
                }
            }
        }

        public AccountNumberListResponse Create(AccountNumberListCreateRequest request)
        {
            ValidateCreate(request);
            _logger.Information(
                "Creating account number list entry for AccountNumber {AccountNumber} and EmployeeId {EmployeeId}.",
                AccountNumberMasker.Mask(request.AccountNumber),
                request.EmployeeId);

            const string sql = @"
INSERT account.AccountNumberList
       (AccountNumber,
        EmployeeId,
        Manual,
        Notes,
        CreatedDate,
        CreatedBy)
OUTPUT inserted.AccountNumberListId,
       inserted.AccountNumber,
       inserted.EmployeeId,
       inserted.Manual,
       inserted.Notes,
       inserted.CreatedDate,
       inserted.CreatedBy,
       inserted.ModifiedDate,
       inserted.ModifiedBy
VALUES (@AccountNumber,
        @EmployeeId,
        @Manual,
        @Notes,
        SYSUTCDATETIME(),
        @CreatedBy);";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumber", request.AccountNumber);
                AddParameter(command, "@EmployeeId", request.EmployeeId);
                AddParameter(command, "@Manual", request.Manual);
                AddParameter(command, "@Notes", NullIfWhiteSpace(request.Notes));
                AddParameter(command, "@CreatedBy", request.CreatedBy);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapAccountNumberList(reader);
                    }
                }
            }

            throw new InvalidOperationException("Account number was not created.");
        }

        public AccountNumberListResponse Update(AccountNumberListUpdateRequest request)
        {
            ValidateUpdate(request);
            _logger.Information(
                "Updating account number list entry {AccountNumberListId}.",
                request.AccountNumberListId);

            const string sql = @"
UPDATE account.AccountNumberList
SET AccountNumber = @AccountNumber,
    EmployeeId = @EmployeeId,
    Manual = @Manual,
    Notes = @Notes,
    ModifiedDate = SYSUTCDATETIME(),
    ModifiedBy = @ModifiedBy
OUTPUT inserted.AccountNumberListId,
       inserted.AccountNumber,
       inserted.EmployeeId,
       inserted.Manual,
       inserted.Notes,
       inserted.CreatedDate,
       inserted.CreatedBy,
       inserted.ModifiedDate,
       inserted.ModifiedBy
WHERE AccountNumberListId = @AccountNumberListId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumberListId", request.AccountNumberListId);
                AddParameter(command, "@AccountNumber", request.AccountNumber);
                AddParameter(command, "@EmployeeId", request.EmployeeId);
                AddParameter(command, "@Manual", request.Manual);
                AddParameter(command, "@Notes", NullIfWhiteSpace(request.Notes));
                AddParameter(command, "@ModifiedBy", request.ModifiedBy);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read()
                        ? MapAccountNumberList(reader)
                        : null;
                }
            }
        }

        public bool Delete(Guid accountNumberListId)
        {
            _logger.Information(
                "Deleting account number list entry {AccountNumberListId}.",
                accountNumberListId);

            const string sql = @"
DELETE account.AccountNumberList
WHERE AccountNumberListId = @AccountNumberListId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@AccountNumberListId", accountNumberListId);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        private IDbConnection CreateConnection()
        {
            var connectionString = _database.Options?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "The ChuA.DatabaseLegacy connection string is not configured.");
            }

            return new SqlConnection(connectionString);
        }

        private static AccountNumberListResponse MapAccountNumberList(IDataRecord record)
        {
            return new AccountNumberListResponse
            {
                AccountNumberListId = record.GetGuid(record.GetOrdinal("AccountNumberListId")),
                AccountNumber = record.GetString(record.GetOrdinal("AccountNumber")),
                EmployeeId = record.GetString(record.GetOrdinal("EmployeeId")),
                Manual = record.GetBoolean(record.GetOrdinal("Manual")),
                Notes = GetNullableString(record, "Notes"),
                CreatedDate = record.GetDateTime(record.GetOrdinal("CreatedDate")),
                CreatedBy = record.GetString(record.GetOrdinal("CreatedBy")),
                ModifiedDate = GetNullableDateTime(record, "ModifiedDate"),
                ModifiedBy = GetNullableString(record, "ModifiedBy")
            };
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

        private static void ValidateCreate(AccountNumberListCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            RequireString(request.AccountNumber, nameof(request.AccountNumber), 16);
            RequireString(request.EmployeeId, nameof(request.EmployeeId), 11);
            RequireString(request.CreatedBy, nameof(request.CreatedBy), 50);
            OptionalString(request.Notes, nameof(request.Notes), 500);
        }

        private static void ValidateUpdate(AccountNumberListUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.AccountNumberListId == Guid.Empty)
            {
                throw new ArgumentException("AccountNumberListId is required.");
            }

            RequireString(request.AccountNumber, nameof(request.AccountNumber), 16);
            RequireString(request.EmployeeId, nameof(request.EmployeeId), 11);
            RequireString(request.ModifiedBy, nameof(request.ModifiedBy), 50);
            OptionalString(request.Notes, nameof(request.Notes), 500);
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
