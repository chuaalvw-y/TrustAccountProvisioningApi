using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ChuA.DatabaseLegacy;
using ChuA.ObservabilityLegacy.Abstractions;
using TrustAccountProvisioningApi.Models;

namespace TrustAccountProvisioningApi.Services
{
    public class NameAndAddressService : INameAndAddressService
    {
        private const string SelectColumns = @"
NameAndAddressId,
CustomerName,
AddressLine1,
AddressLine2,
AddressLine3,
AddressLine4,
AddressLine5,
City,
PostalCode,
State,
PhoneNumber,
Salutation,
Email,
DateOfBirth,
CreatedDate,
ModifiedDate,
IsActive";

        private readonly IChuADatabase _database;
        private readonly IApplicationLogger _logger;

        public NameAndAddressService(
            IChuADatabase database,
            IApplicationLogger logger)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<NameAndAddressResponse> Search(NameAndAddressSearchRequest request)
        {
            request = request ?? new NameAndAddressSearchRequest();
            _logger.Information(
                "Searching name and address records for City {City}, State {State}, PostalCode {PostalCode}, and IsActive {IsActive}.",
                request.City,
                request.State,
                request.PostalCode,
                request.IsActive);

            var sql = $@"
SELECT {SelectColumns}
FROM client.NameAndAddress
WHERE (@CustomerName IS NULL OR CustomerName LIKE '%' + @CustomerName + '%')
  AND (@City IS NULL OR City LIKE @City + '%')
  AND (@PostalCode IS NULL OR PostalCode LIKE @PostalCode + '%')
  AND (@State IS NULL OR State = @State)
  AND (@IsActive IS NULL OR IsActive = @IsActive)
ORDER BY CreatedDate DESC;";

            var rows = new List<NameAndAddressResponse>();

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@CustomerName", NullIfWhiteSpace(request.CustomerName));
                AddParameter(command, "@City", NullIfWhiteSpace(request.City));
                AddParameter(command, "@PostalCode", NullIfWhiteSpace(request.PostalCode));
                AddParameter(command, "@State", NullIfWhiteSpace(request.State));
                AddParameter(command, "@IsActive", request.IsActive);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(MapNameAndAddress(reader));
                    }
                }
            }

            return rows;
        }

        public NameAndAddressResponse Get(Guid nameAndAddressId)
        {
            _logger.Information(
                "Getting name and address record {NameAndAddressId}.",
                nameAndAddressId);

            var sql = $@"
SELECT {SelectColumns}
FROM client.NameAndAddress
WHERE NameAndAddressId = @NameAndAddressId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@NameAndAddressId", nameAndAddressId);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapNameAndAddress(reader) : null;
                }
            }
        }

        public NameAndAddressResponse Create(NameAndAddressCreateRequest request)
        {
            ValidateCreate(request);
            _logger.Information(
                "Creating name and address record for City {City}, State {State}, and PostalCode {PostalCode}.",
                request.City,
                request.State,
                request.PostalCode);

            var sql = $@"
INSERT client.NameAndAddress
       (NameAndAddressId, CustomerName, AddressLine1, AddressLine2, AddressLine3,
        AddressLine4, AddressLine5, City, PostalCode, State, PhoneNumber,
        Salutation, Email, DateOfBirth, CreatedDate, IsActive)
OUTPUT {SqlColumnList.ToInsertedColumns(SelectColumns)}
VALUES (@NameAndAddressId, @CustomerName, @AddressLine1, @AddressLine2, @AddressLine3,
        @AddressLine4, @AddressLine5, @City, @PostalCode, @State, @PhoneNumber,
        @Salutation, @Email, @DateOfBirth, SYSUTCDATETIME(), @IsActive);";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@NameAndAddressId", Guid.NewGuid());
                AddRequestParameters(command, request);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapNameAndAddress(reader);
                    }
                }
            }

            throw new InvalidOperationException("Name and address record was not created.");
        }

        public NameAndAddressResponse Update(NameAndAddressUpdateRequest request)
        {
            ValidateUpdate(request);
            _logger.Information(
                "Updating name and address record {NameAndAddressId}.",
                request.NameAndAddressId);

            var sql = $@"
UPDATE client.NameAndAddress
SET CustomerName = @CustomerName,
    AddressLine1 = @AddressLine1,
    AddressLine2 = @AddressLine2,
    AddressLine3 = @AddressLine3,
    AddressLine4 = @AddressLine4,
    AddressLine5 = @AddressLine5,
    City = @City,
    PostalCode = @PostalCode,
    State = @State,
    PhoneNumber = @PhoneNumber,
    Salutation = @Salutation,
    Email = @Email,
    DateOfBirth = @DateOfBirth,
    ModifiedDate = SYSUTCDATETIME(),
    IsActive = @IsActive
OUTPUT {SqlColumnList.ToInsertedColumns(SelectColumns)}
WHERE NameAndAddressId = @NameAndAddressId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@NameAndAddressId", request.NameAndAddressId);
                AddRequestParameters(command, request);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapNameAndAddress(reader) : null;
                }
            }
        }

        public bool Delete(Guid nameAndAddressId)
        {
            _logger.Information(
                "Deleting name and address record {NameAndAddressId}.",
                nameAndAddressId);

            const string sql = @"
DELETE client.NameAndAddress
WHERE NameAndAddressId = @NameAndAddressId;";

            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParameter(command, "@NameAndAddressId", nameAndAddressId);

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

        private static void AddRequestParameters(IDbCommand command, NameAndAddressCreateRequest request)
        {
            AddParameter(command, "@CustomerName", request.CustomerName);
            AddParameter(command, "@AddressLine1", request.AddressLine1);
            AddParameter(command, "@AddressLine2", NullIfWhiteSpace(request.AddressLine2));
            AddParameter(command, "@AddressLine3", NullIfWhiteSpace(request.AddressLine3));
            AddParameter(command, "@AddressLine4", NullIfWhiteSpace(request.AddressLine4));
            AddParameter(command, "@AddressLine5", NullIfWhiteSpace(request.AddressLine5));
            AddParameter(command, "@City", request.City);
            AddParameter(command, "@PostalCode", request.PostalCode);
            AddParameter(command, "@State", request.State);
            AddParameter(command, "@PhoneNumber", NullIfWhiteSpace(request.PhoneNumber));
            AddParameter(command, "@Salutation", NullIfWhiteSpace(request.Salutation));
            AddParameter(command, "@Email", NullIfWhiteSpace(request.Email));
            AddParameter(command, "@DateOfBirth", request.DateOfBirth);
            AddParameter(command, "@IsActive", request.IsActive);
        }

        private static NameAndAddressResponse MapNameAndAddress(IDataRecord record)
        {
            return new NameAndAddressResponse
            {
                NameAndAddressId = record.GetGuid(record.GetOrdinal("NameAndAddressId")),
                CustomerName = record.GetString(record.GetOrdinal("CustomerName")),
                AddressLine1 = record.GetString(record.GetOrdinal("AddressLine1")),
                AddressLine2 = GetNullableString(record, "AddressLine2"),
                AddressLine3 = GetNullableString(record, "AddressLine3"),
                AddressLine4 = GetNullableString(record, "AddressLine4"),
                AddressLine5 = GetNullableString(record, "AddressLine5"),
                City = record.GetString(record.GetOrdinal("City")),
                PostalCode = record.GetString(record.GetOrdinal("PostalCode")),
                State = record.GetString(record.GetOrdinal("State")),
                PhoneNumber = GetNullableString(record, "PhoneNumber"),
                Salutation = GetNullableString(record, "Salutation"),
                Email = GetNullableString(record, "Email"),
                DateOfBirth = GetNullableDateTime(record, "DateOfBirth"),
                CreatedDate = record.GetDateTime(record.GetOrdinal("CreatedDate")),
                ModifiedDate = GetNullableDateTime(record, "ModifiedDate"),
                IsActive = record.GetBoolean(record.GetOrdinal("IsActive"))
            };
        }

        private static void ValidateCreate(NameAndAddressCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateRequest(request);
        }

        private static void ValidateUpdate(NameAndAddressUpdateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.NameAndAddressId == Guid.Empty)
            {
                throw new ArgumentException("NameAndAddressId is required.");
            }

            ValidateRequest(request);
        }

        private static void ValidateRequest(NameAndAddressCreateRequest request)
        {
            RequireString(request.CustomerName, nameof(request.CustomerName), 100);
            RequireString(request.AddressLine1, nameof(request.AddressLine1), 36);
            RequireString(request.City, nameof(request.City), 40);
            RequireString(request.PostalCode, nameof(request.PostalCode), 10);
            RequireString(request.State, nameof(request.State), 4);
            OptionalString(request.AddressLine2, nameof(request.AddressLine2), 36);
            OptionalString(request.AddressLine3, nameof(request.AddressLine3), 36);
            OptionalString(request.AddressLine4, nameof(request.AddressLine4), 36);
            OptionalString(request.AddressLine5, nameof(request.AddressLine5), 36);
            OptionalString(request.PhoneNumber, nameof(request.PhoneNumber), 25);
            OptionalString(request.Salutation, nameof(request.Salutation), 50);
            OptionalString(request.Email, nameof(request.Email), 50);
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
