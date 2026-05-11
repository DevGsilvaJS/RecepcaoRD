using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class AuditoriaRdRecepcaoConfiguracao : IEntityTypeConfiguration<AuditoriaRdRecepcao>
{
  public void Configure(EntityTypeBuilder<AuditoriaRdRecepcao> builder)
  {
    builder.ToTable("SGT_AUDITORIA_RD_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("ARD_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.IdReferenciaRegistroAuditado)
      .HasColumnName("ARD_ID_REFERENCIA")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.CpfCnpjUsuario)
      .HasColumnName("ARD_CPF_CNPJ_USUARIO")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.NomeUsuario)
      .HasColumnName("ARD_NOME_USUARIO")
      .HasColumnType("VARCHAR2(155)")
      .HasMaxLength(155)
      .IsRequired();

    builder.Property(x => x.CnpjEmpresaRepresentada)
      .HasColumnName("ARD_CNPJ_EMPRESA")
      .HasColumnType("VARCHAR2(14)")
      .HasMaxLength(14)
      .IsRequired();

    builder.Property(x => x.NomeEmpresaRepresentada)
      .HasColumnName("ARD_NOME_EMPRESA")
      .HasColumnType("VARCHAR2(155)")
      .HasMaxLength(155)
      .IsRequired();

    builder.Property(x => x.TipoAcao)
      .HasColumnName("ARD_TIPO_ACAO")
      .HasColumnType("VARCHAR2(1)")
      .HasMaxLength(1)
      .IsRequired();

    builder.Property(x => x.DataHoraAcao)
      .HasColumnName("ARD_DATA_HORA_ACAO")
      .HasColumnType("TIMESTAMP")
      .IsRequired();

    builder.Property(x => x.DescricaoAcao)
      .HasColumnName("ARD_DESCRICAO_ACAO")
      .HasColumnType("VARCHAR2(1000)")
      .HasMaxLength(1000)
      .IsRequired();

    builder.Property(x => x.FuncionalidadeId)
      .HasColumnName("LFU_ID")
      .HasColumnType("NUMBER(4,0)")
      .IsRequired();

    builder
      .HasOne(x => x.Funcionalidade)
      .WithMany(x => x.Auditorias)
      .HasForeignKey(x => x.FuncionalidadeId)
      .HasConstraintName("FK_ARD_LFU")
      .OnDelete(DeleteBehavior.NoAction);
  }
}
