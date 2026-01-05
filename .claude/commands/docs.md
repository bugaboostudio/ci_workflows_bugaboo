---
description: Atualizar documentação do projeto
---

Você deve ajudar a atualizar e manter a documentação do projeto.

## Processo

1. **Identificar o que documentar**:
   - Perguntar ao usuário: "O que deseja documentar?"
     - Nova feature implementada
     - API changes
     - Setup instructions
     - Troubleshooting
     - Architecture decisions

2. **Arquivos de documentação**:
   - **README.md**: Overview do projeto, setup, como rodar
   - **CLAUDE.md**: Guia detalhado para AI e desenvolvedores
   - **CHANGELOG.md**: Histórico de mudanças (se existir)
   - **Code comments**: Documentação inline

3. **README.md** (raiz do projeto):
   ```markdown
   # Project Name

   ## Descrição
   [O que é o projeto]

   ## Features
   - Feature 1
   - Feature 2

   ## Setup
   1. Clonar repositório
   2. Abrir na Unity
   3. Instalar dependências

   ## Como Rodar
   [Instruções]

   ## Testes
   [Como rodar testes]

   ## Build
   [Como fazer build]

   ## Contribuindo
   [Guidelines]

   ## License
   [License info]
   ```

4. **CLAUDE.md** (para AI assistants):
   - Atualizar se arquitetura mudou
   - Adicionar novas convenções
   - Documentar novos workflows
   - Atualizar estrutura de pastas

5. **Code Documentation**:
   - Adicionar XML comments em public APIs:
     ```csharp
     /// <summary>
     /// Calcula dano baseado em ataque e defesa.
     /// </summary>
     /// <param name="attack">Valor de ataque</param>
     /// <param name="defense">Valor de defesa</param>
     /// <returns>Dano final calculado</returns>
     public int CalculateDamage(int attack, int defense)
     {
         return Mathf.Max(0, attack - defense);
     }
     ```

6. **CHANGELOG.md** (se não existir, criar):
   ```markdown
   # Changelog

   ## [Unreleased]
   ### Added
   - Nova feature X

   ### Changed
   - Mudança Y

   ### Fixed
   - Bug Z

   ## [1.0.0] - 2026-01-05
   ### Added
   - Initial release
   ```

7. **Diagrams** (se necessário):
   - Usar Mermaid para diagramas em markdown
   - Architecture diagrams
   - Flow charts
   - Sequence diagrams

8. **Verificar e commitar**:
   ```bash
   git add README.md CLAUDE.md
   git commit -m "docs: atualiza documentação com [descrição]"
   ```

## Templates

### API Documentation
```csharp
/// <summary>
/// [Descrição breve]
/// </summary>
/// <remarks>
/// [Detalhes adicionais, comportamento especial, etc.]
/// </remarks>
/// <example>
/// <code>
/// // Exemplo de uso
/// var result = MyMethod(param);
/// </code>
/// </example>
```

### Architecture Decision Record
```markdown
# ADR-001: [Título da Decisão]

## Status
Accepted / Rejected / Deprecated

## Context
[Por que essa decisão foi necessária]

## Decision
[O que foi decidido]

## Consequences
[Implicações da decisão]
```

Use bom senso para documentar o suficiente sem over-document. Foque em WHY, não apenas WHAT.
