using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class PlanoRecepcaoConfiguracao : IEntityTypeConfiguration<PlanoRecepcao>
{
  public void Configure(EntityTypeBuilder<PlanoRecepcao> builder)
  {
    builder.ToTable("SGT_PLANO_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("PLA_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.CnpjEmpresa)
      .HasColumnName("PLA_CNPJ_EMPRESA")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.NomeRepresentante)
      .HasColumnName("PLA_NOME_REPRESENTANTE")
      .HasColumnType("VARCHAR2(100)")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.CargoRepresentante)
      .HasColumnName("PLA_CARGO_REPRESENTANTE")
      .HasColumnType("VARCHAR2(50)")
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(x => x.EmailRepresentante)
      .HasColumnName("PLA_EMAIL_REPRESENTANTE")
      .HasColumnType("VARCHAR2(100)")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.FoneRepresentante)
      .HasColumnName("PLA_FONE_REPRESENTANTE")
      .HasColumnType("VARCHAR2(20)")
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(x => x.DataInicioVigencia)
      .HasColumnName("PLA_DATA_INICIO_VIGENCIA")
      .HasColumnType("DATE")
      .IsRequired();

    builder.Property(x => x.DataFimVigencia)
      .HasColumnName("PLA_DATA_FIM_VIGENCIA")
      .HasColumnType("DATE")
      .IsRequired();

    builder.Property(x => x.NumeroProtocolo)
      .HasColumnName("PLA_NUMERO_PROTOCOLO")
      .HasColumnType("VARCHAR2(20)")
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(x => x.DataProtocolo)
      .HasColumnName("PLA_DATA_PROTOCOLO")
      .HasColumnType("DATE")
      .IsRequired();

    builder.Property(x => x.NumeroProcesso)
      .HasColumnName("PLA_NUMERO_PROCESSO")
      .HasColumnType("VARCHAR2(9)")
      .HasMaxLength(9)
      .IsRequired();

    builder.Property(x => x.NumeroPlano)
      .HasColumnName("PLA_NUMERO_PLANO")
      .HasColumnType("VARCHAR2(11)")
      .HasMaxLength(11)
      .IsRequired();

    builder.Property(x => x.Situacao)
      .HasColumnName("PLA_SITUACAO")
      .HasColumnType("NUMBER(1)")
      .HasConversion<int>()
      .IsRequired();
  }
}
