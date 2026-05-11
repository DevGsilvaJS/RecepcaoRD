using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class AnexosRecepcaoConfiguracao : IEntityTypeConfiguration<AnexosRecepcao>
{
  public void Configure(EntityTypeBuilder<AnexosRecepcao> builder)
  {
    builder.ToTable("SGT_ANEXOS_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("ANR_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.IdReferenciaRegistroPai)
      .HasColumnName("ANR_ID_REFERENCIA")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.TipoAnexoId)
      .HasColumnName("TPA_ID")
      .HasColumnType("NUMBER(38,0)")
      .IsRequired();

    builder.Property(x => x.FuncionalidadeId)
      .HasColumnName("LFU_ID")
      .HasColumnType("NUMBER(4,0)")
      .IsRequired();

    builder.Property(x => x.NomeArquivo)
      .HasColumnName("ANR_NOME_ARQUIVO")
      .HasColumnType("VARCHAR2(155)")
      .HasMaxLength(155)
      .IsRequired();

    builder.Property(x => x.Arquivo)
      .HasColumnName("ANR_ARQUIVO")
      .HasColumnType("BLOB");

    builder
      .HasOne(x => x.TipoAnexo)
      .WithMany(x => x.Anexos)
      .HasForeignKey(x => x.TipoAnexoId)
      .HasConstraintName("FK_ANR_TPA")
      .OnDelete(DeleteBehavior.NoAction);

    builder
      .HasOne(x => x.Funcionalidade)
      .WithMany(x => x.Anexos)
      .HasForeignKey(x => x.FuncionalidadeId)
      .HasConstraintName("FK_ANR_LFU")
      .OnDelete(DeleteBehavior.NoAction);
  }
}
