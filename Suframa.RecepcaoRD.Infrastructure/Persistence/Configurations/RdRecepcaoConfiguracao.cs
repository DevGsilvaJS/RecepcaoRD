using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class RdRecepcaoConfiguracao : IEntityTypeConfiguration<RdRecepcao>
{
  public void Configure(EntityTypeBuilder<RdRecepcao> builder)
  {
    builder.ToTable("SGT_RD_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("RDR_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.Cnpj)
      .HasColumnName("RDR_CNPJ")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.RazaoSocial)
      .HasColumnName("RDR_RAZAO_SOCIAL")
      .HasColumnType("VARCHAR2(155)")
      .HasMaxLength(155)
      .IsRequired();

    builder.Property(x => x.TipoRd)
      .HasColumnName("RDR_TIPO_RD")
      .HasColumnType("NUMBER(1)")
      .HasConversion<int>()
      .IsRequired();

    builder.Property(x => x.IdOrigem)
      .HasColumnName("RDR_ID_ORIGEM")
      .HasColumnType("NUMBER(38,0)");

    builder.Property(x => x.InscricaoSuframa)
      .HasColumnName("RDR_INSCRICAO_SUFRAMA")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.EnderecoEmpresa)
      .HasColumnName("RDR_ENDERECO_EMPRESA")
      .HasColumnType("VARCHAR2(255)")
      .HasMaxLength(255)
      .IsRequired();

    builder.Property(x => x.RepresentanteLegal)
      .HasColumnName("RDR_REPRESENTANTE_LEGAL")
      .HasColumnType("VARCHAR2(100)")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.Telefone)
      .HasColumnName("RDR_TELEFONE")
      .HasColumnType("VARCHAR2(20)")
      .HasMaxLength(20)
      .IsRequired();

    builder.Property(x => x.Email)
      .HasColumnName("RDR_EMAIL")
      .HasColumnType("VARCHAR2(255)")
      .HasMaxLength(255)
      .IsRequired();

    builder.Property(x => x.AnoCalendario)
      .HasColumnName("RDR_ANO_CALENDARIO")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.PlanoId)
      .HasColumnName("PLA_ID")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.NumeroDeclaracaoVeracidade)
      .HasColumnName("RDR_NUM_DEC_VERACIDADE")
      .HasColumnType("VARCHAR2(30)")
      .HasMaxLength(30);

    builder.Property(x => x.DataEmissao)
      .HasColumnName("RDR_DATA_EMISSAO")
      .HasColumnType("DATE");

    builder.Property(x => x.DataCriacao)
      .HasColumnName("RDR_DATA_CRIACAO")
      .HasColumnType("TIMESTAMP")
      .HasDefaultValueSql("SYSTIMESTAMP")
      .IsRequired();

    builder
      .HasOne(x => x.Plano)
      .WithMany(x => x.Rds)
      .HasForeignKey(x => x.PlanoId)
      .HasConstraintName("FK_RDR_PLA")
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasOne(x => x.RdOrigem)
      .WithMany()
      .HasForeignKey(x => x.IdOrigem)
      .HasConstraintName("FK_RDR_RDR_ORIGEM")
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasMany(x => x.RelatoriosAuditoria)
      .WithOne(x => x.RdRecepcao)
      .HasForeignKey(x => x.RdRecepcaoId)
      .HasConstraintName("FK_RAU_RDR")
      .OnDelete(DeleteBehavior.NoAction);
  }
}
