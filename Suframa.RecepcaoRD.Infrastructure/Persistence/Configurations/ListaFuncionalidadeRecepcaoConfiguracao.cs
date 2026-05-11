using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Configurations;

public sealed class ListaFuncionalidadeRecepcaoConfiguracao : IEntityTypeConfiguration<ListaFuncionalidadeRecepcao>
{
  public void Configure(EntityTypeBuilder<ListaFuncionalidadeRecepcao> builder)
  {
    builder.ToTable("SGT_LISTA_FUNCIONALIDADE_RECEPCAO", "RECEPCAORD");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
      .HasColumnName("LFU_ID")
      .HasColumnType("NUMBER(4,0)")
      .ValueGeneratedOnAdd();

    builder.Property(x => x.Descricao)
      .HasColumnName("LFU_DESCRICAO")
      .HasColumnType("VARCHAR2(255)")
      .HasMaxLength(255)
      .IsRequired();

    builder.Property(x => x.NomeTabela)
      .HasColumnName("LFU_NOME_TABELA")
      .HasColumnType("VARCHAR2(100)")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(x => x.IdVinculoFuncionalidadePai)
      .HasColumnName("LFU_ID_VINCULO")
      .HasColumnType("NUMBER(4,0)");

    builder
      .HasOne(x => x.FuncionalidadePai)
      .WithMany(x => x.FuncionalidadesFilhas)
      .HasForeignKey(x => x.IdVinculoFuncionalidadePai)
      .HasConstraintName("FK_LFU_LFU_PAI")
      .OnDelete(DeleteBehavior.NoAction);
  }
}
