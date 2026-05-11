using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class RelatorioAuditoriaRecepcaoConfiguracao : IEntityTypeConfiguration<RelatorioAuditoriaRecepcao>
{
  public void Configure(EntityTypeBuilder<RelatorioAuditoriaRecepcao> builder)
  {
    builder.ToTable("SGT_RELATORIO_AUDITORIA_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("RAU_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.RdRecepcaoId)
      .HasColumnName("RDR_ID")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.RazaoSocialFirma)
      .HasColumnName("RAU_RAZAO_SOCIAL")
      .HasColumnType("VARCHAR2(155)")
      .HasMaxLength(155)
      .IsRequired();

    builder.Property(x => x.CnpjFirma)
      .HasColumnName("RAU_CNPJ")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.DataRelatorio)
      .HasColumnName("RAU_DATA")
      .HasColumnType("DATE")
      .IsRequired();

    builder.Property(x => x.Situacao)
      .HasColumnName("RAU_SITUACAO")
      .HasColumnType("NUMBER(1)")
      .HasConversion<int>()
      .IsRequired();

    builder.Property(x => x.Observacao)
      .HasColumnName("RAU_OBSERVACAO")
      .HasColumnType("CLOB");

    builder.Property(x => x.IdPredecessor)
      .HasColumnName("RAU_ID_PREDECESSOR")
      .HasColumnType("NUMBER(38,0)");

    builder.Property(x => x.IdReferenciaAnalise)
      .HasColumnName("RAU_ID_REFERENCIA")
      .HasColumnType("NUMBER(38,0)");

    builder.Property(x => x.Alterado)
      .HasColumnName("RAU_ALTERADO")
      .HasColumnType("NUMBER(1)");

    builder
      .HasOne(x => x.RdRecepcao)
      .WithMany(x => x.RelatoriosAuditoria)
      .HasForeignKey(x => x.RdRecepcaoId)
      .HasConstraintName("FK_RAU_RDR")
      .OnDelete(DeleteBehavior.NoAction);
  }
}
