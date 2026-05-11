using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class TiposAnexosRecepcaoConfiguracao : IEntityTypeConfiguration<TiposAnexosRecepcao>
{
  public void Configure(EntityTypeBuilder<TiposAnexosRecepcao> builder)
  {
    builder.ToTable("SGT_TIPOS_ANEXOS_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("TPA_ID")
      .HasColumnType("NUMBER(38,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.Nome)
      .HasColumnName("TPA_NOME")
      .HasColumnType("VARCHAR2(50)")
      .HasMaxLength(50)
      .IsRequired();
  }
}
