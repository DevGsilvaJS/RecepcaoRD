using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Suframa.RecepcaoRD.Application.Abstractions.Cadsuf;
using Suframa.RecepcaoRD.Application.DTOs.Cadsuf;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Repositories;

public sealed class CadsufCompanyRepository : ICadsufCompanyRepository
{
  private readonly CadsufDbContext _dbContext;

  public CadsufCompanyRepository(CadsufDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<CadsufCompanyRegistrationVm?> GetCompanyByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(cnpj))
    {
      return null;
    }

    const string sql = """
SELECT
  vic.PJU_CO_CNPJ AS CNPJ,
  CAST(vic.INS_CO AS VARCHAR(14)) AS INSCRICAO_SUFRAMA,
  vic.PJU_DS_RAZAO_SOCIAL AS RAZAO_SOCIAL,
  vic.CEP_TP_LOGRADOURO + ' ' +
  CASE
    WHEN vic.CEP_DS_ENDERECO LIKE vic.CEP_TP_LOGRADOURO + '%'
    THEN SUBSTRING(vic.CEP_DS_ENDERECO, LEN(vic.CEP_TP_LOGRADOURO) + 1, LEN(vic.CEP_DS_ENDERECO))
    ELSE vic.CEP_DS_ENDERECO
  END + ', ' +
  vic.PJU_NU_ENDERECO +
  COALESCE(' ' + vic.PJU_DS_COMPLEMENTO, '') +
  CASE WHEN vic.CEP_DS_BAIRRO IS NOT NULL THEN ' - ' END +
  vic.CEP_DS_BAIRRO + ' - CEP: ' +
  SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 1, 2) + '.' + SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 3, 3) + '-' + SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 6, 3) + ' - ' +
  vic.MUN_DS + ', ' +
  vic.MUN_SG_UF
  AS ENDERECO
FROM dbh_10306_cadsuf.dbo.VW_INSCRICAO_CADASTRAL vic
WHERE vic.PJU_CO_CNPJ = @cnpj;
""";

    await using DbConnection connection = _dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
      await connection.OpenAsync(cancellationToken);
    }

    await using DbCommand command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandType = CommandType.Text;

    DbParameter parameter = command.CreateParameter();
    parameter.ParameterName = "@cnpj";
    parameter.Value = cnpj.Trim();
    command.Parameters.Add(parameter);

    await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return new CadsufCompanyRegistrationVm(
      GetString(reader, "CNPJ"),
      GetString(reader, "INSCRICAO_SUFRAMA"),
      GetString(reader, "RAZAO_SOCIAL"),
      GetString(reader, "ENDERECO"));
  }

  public async Task<CadsufCompanyRegistrationVm?> GetCompanyByCpfAsync(string cpfSomenteDigitos, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(cpfSomenteDigitos) || cpfSomenteDigitos.Length != 11)
    {
      return null;
    }

    const string sql = """
SELECT TOP (1)
  vic.PJU_CO_CNPJ AS CNPJ,
  CAST(vic.INS_CO AS VARCHAR(14)) AS INSCRICAO_SUFRAMA,
  vic.PJU_DS_RAZAO_SOCIAL AS RAZAO_SOCIAL,
  vic.CEP_TP_LOGRADOURO + ' ' +
  CASE
    WHEN vic.CEP_DS_ENDERECO LIKE vic.CEP_TP_LOGRADOURO + '%'
    THEN SUBSTRING(vic.CEP_DS_ENDERECO, LEN(vic.CEP_TP_LOGRADOURO) + 1, LEN(vic.CEP_DS_ENDERECO))
    ELSE vic.CEP_DS_ENDERECO
  END + ', ' +
  vic.PJU_NU_ENDERECO +
  COALESCE(' ' + vic.PJU_DS_COMPLEMENTO, '') +
  CASE WHEN vic.CEP_DS_BAIRRO IS NOT NULL THEN ' - ' END +
  vic.CEP_DS_BAIRRO + ' - CEP: ' +
  SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 1, 2) + '.' + SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 3, 3) + '-' + SUBSTRING(CAST(vic.CEP_CO AS VARCHAR(8)), 6, 3) + ' - ' +
  vic.MUN_DS + ', ' +
  vic.MUN_SG_UF
  AS ENDERECO
FROM dbh_10306_cadsuf.dbo.VW_INSCRICAO_CADASTRAL vic
WHERE REPLACE(REPLACE(REPLACE(REPLACE(ISNULL(vic.PFU_CO_CPF, N''), N'.', N''), N'-', N''), N'/', N''), N' ', N'') = @cpf;
""";

    await using DbConnection connection = _dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
      await connection.OpenAsync(cancellationToken);
    }

    await using DbCommand command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandType = CommandType.Text;

    DbParameter parameter = command.CreateParameter();
    parameter.ParameterName = "@cpf";
    parameter.Value = cpfSomenteDigitos.Trim();
    command.Parameters.Add(parameter);

    await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return new CadsufCompanyRegistrationVm(
      GetString(reader, "CNPJ"),
      GetString(reader, "INSCRICAO_SUFRAMA"),
      GetString(reader, "RAZAO_SOCIAL"),
      GetString(reader, "ENDERECO"));
  }

  private static string GetString(DbDataReader reader, string column)
  {
    int ordinal = reader.GetOrdinal(column);
    return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
  }
}

