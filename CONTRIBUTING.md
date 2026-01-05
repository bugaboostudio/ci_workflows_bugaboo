# Guia de Contribuição

Obrigado por considerar contribuir com este template Unity! 🎮

Este documento fornece guidelines para contribuir com melhorias neste template que serve como base para projetos Unity na Bugaboo Studio.

## 📋 Índice

- [Código de Conduta](#código-de-conduta)
- [Como Posso Contribuir?](#como-posso-contribuir)
- [Processo de Desenvolvimento](#processo-de-desenvolvimento)
- [GitFlow Workflow](#gitflow-workflow)
- [Conventional Commits](#conventional-commits)
- [Pull Request Process](#pull-request-process)
- [Style Guide](#style-guide)
- [Estrutura do Projeto](#estrutura-do-projeto)

---

## Código de Conduta

Este projeto adere a um código de conduta profissional. Ao participar, você concorda em manter um ambiente respeitoso e colaborativo.

### Nossas Expectativas

- Use linguagem acolhedora e inclusiva
- Respeite diferentes pontos de vista e experiências
- Aceite críticas construtivas de forma profissional
- Foque no que é melhor para a comunidade
- Mostre empatia com outros membros da comunidade

---

## Como Posso Contribuir?

### 🐛 Reportar Bugs

Antes de criar um bug report:

1. Verifique se o bug já não foi reportado em [Issues](https://github.com/bugaboostudio/ci_workflows_bugaboo/issues)
2. Determine qual parte do template está causando o problema
3. Colete informações sobre o ambiente (Unity version, OS, etc.)

**Como criar um bom bug report:**

```markdown
**Descrição do Bug**
Uma descrição clara e concisa do problema.

**Como Reproduzir**
Passos para reproduzir o comportamento:
1. Vá para '...'
2. Clique em '...'
3. Execute '...'
4. Veja o erro

**Comportamento Esperado**
O que deveria acontecer.

**Screenshots**
Se aplicável, adicione screenshots.

**Ambiente:**
- OS: [ex: Windows 11, macOS Sonoma]
- Unity Version: [ex: 2022.3.10f1]
- Template Version: [ex: 1.0.0]

**Contexto Adicional**
Qualquer outra informação relevante.
```

### 💡 Sugerir Melhorias

Melhorias podem incluir:

- Novos workflows de CI/CD
- Otimizações de performance
- Melhorias na documentação
- Novos scripts utilitários
- Novas configurações de quality

**Template para sugestão:**

```markdown
**A melhoria está relacionada a um problema?**
Descrição clara do problema. Ex: "É frustrante quando..."

**Descreva a solução que você gostaria**
Descrição clara e concisa da solução proposta.

**Alternativas consideradas**
Outras soluções ou features que você considerou.

**Contexto adicional**
Screenshots, exemplos de outros projetos, etc.
```

### 📝 Melhorar Documentação

Documentação sempre pode melhorar! Contribuições são bem-vindas para:

- Corrigir typos ou erros
- Adicionar exemplos práticos
- Traduzir documentação
- Clarificar instruções confusas
- Adicionar diagramas ou imagens

### 🔧 Contribuir com Código

Siga o [Processo de Desenvolvimento](#processo-de-desenvolvimento) abaixo.

---

## Processo de Desenvolvimento

### 1. Fork & Clone

```bash
# Fork no GitHub UI primeiro
git clone https://github.com/SEU-USERNAME/ci_workflows_bugaboo.git
cd ci_workflows_bugaboo
git remote add upstream https://github.com/bugaboostudio/ci_workflows_bugaboo.git
```

### 2. Criar Branch

Siga o [GitFlow Workflow](#gitflow-workflow):

```bash
# Para features
git checkout develop
git pull upstream develop
git checkout -b feature/nome-da-feature

# Para hotfixes
git checkout main
git pull upstream main
git checkout -b hotfix/descricao-do-fix
```

### 3. Fazer Mudanças

- Escreva código limpo e bem documentado
- Siga o [Style Guide](#style-guide)
- Adicione/atualize testes se aplicável
- Atualize documentação relevante

### 4. Commit

Use [Conventional Commits](#conventional-commits):

```bash
git add .
git commit -m "feat(workflows): adiciona workflow de deploy Steam"
```

### 5. Push & Pull Request

```bash
git push origin feature/nome-da-feature
```

Depois crie Pull Request no GitHub seguindo o [template de PR](.github/PULL_REQUEST_TEMPLATE.md).

---

## GitFlow Workflow

Este projeto usa GitFlow. Leia a seção completa em [CLAUDE.md](CLAUDE.md#gitflow-workflow-template-best-practices).

### Branches Principais

- **main**: Código de produção (sempre estável)
- **develop**: Branch de integração de desenvolvimento

### Branches de Suporte

- **feature/**: Novas features (origem: develop)
- **release/**: Preparação de release (origem: develop)
- **hotfix/**: Correções urgentes (origem: main)

### Nomenclatura de Branches

- `feature/nome-descritivo`
- `feature/ISSUE-123-descricao`
- `release/v1.2.0`
- `hotfix/v1.2.1` ou `hotfix/correcao-critica`

---

## Conventional Commits

Este projeto segue [Conventional Commits](https://www.conventionalcommits.org/).

### Formato

```
<tipo>(<escopo>): <descrição>

[corpo opcional]

[rodapé opcional]
```

### Tipos

- **feat**: Nova funcionalidade
- **fix**: Correção de bug
- **docs**: Mudanças em documentação
- **style**: Formatação (não afeta código)
- **refactor**: Refatoração de código
- **test**: Adicionar/corrigir testes
- **chore**: Tarefas de manutenção
- **perf**: Melhorias de performance
- **ci**: Mudanças em CI/CD

### Exemplos

```bash
# Feature
git commit -m "feat(workflows): adiciona workflow de build Android"

# Bugfix
git commit -m "fix(cache): corrige invalidação de cache da Library"

# Documentação
git commit -m "docs(readme): atualiza instruções de instalação"

# Breaking change
git commit -m "feat(structure)!: reorganiza estrutura de diretórios

BREAKING CHANGE: Diretórios de Scripts foram reorganizados"

# Com issue
git commit -m "fix(workflows): corrige timeout no build iOS

Closes #123"
```

---

## Pull Request Process

### Checklist Antes de Abrir PR

- [ ] Código segue o style guide do projeto
- [ ] Fiz self-review do código
- [ ] Comentei partes complexas do código
- [ ] Atualizei a documentação relevante
- [ ] Minhas mudanças não geram warnings
- [ ] Adicionei testes (se aplicável)
- [ ] Todos os testes passam localmente
- [ ] Build local passou sem erros
- [ ] Atualizei CHANGELOG.md na seção `[Unreleased]`

### Processo de Review

1. **Automated Checks**: CI deve passar (tests, builds)
2. **Code Review**: Pelo menos 1 aprovação necessária
3. **Discussão**: Responda a comentários e faça ajustes
4. **Merge**: Após aprovação, mantenedor fará merge

### Dicas para Bom PR

- **Título Claro**: Use conventional commits no título
- **Descrição Detalhada**: Explique o que, por quê e como
- **Pequeno e Focado**: Um PR = uma feature/fix
- **Screenshots**: Adicione se houver mudanças visuais
- **Link Issues**: Reference issues relacionadas

---

## Style Guide

### C# (Unity)

Siga [Unity C# Coding Standards](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity):

```csharp
// Classes: PascalCase
public class PlayerController : MonoBehaviour
{
    // Campos públicos: PascalCase
    public float MoveSpeed = 5f;

    // Campos privados: camelCase ou _camelCase
    private float jumpForce = 10f;
    private Transform _playerTransform;

    // Métodos: PascalCase
    public void Jump()
    {
        // Implementação
    }

    // Métodos privados: PascalCase
    private void ApplyGravity()
    {
        // Implementação
    }
}
```

### YAML (Workflows)

```yaml
# Indentação: 2 espaços
name: Build Unity Project

on:
  push:
    branches: [develop, main]

jobs:
  build:
    name: Build for ${{ matrix.platform }}
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v3
```

### Markdown (Documentação)

- Use cabeçalhos hierárquicos (# ## ###)
- Adicione espaço em branco entre seções
- Use code blocks com syntax highlighting
- Adicione links para referências externas
- Use listas para melhor legibilidade

---

## Estrutura do Projeto

### Organização de Diretórios

```
ci_workflows_bugaboo/
├── .github/
│   ├── workflows/        # GitHub Actions workflows
│   ├── CODEOWNERS        # Code ownership
│   ├── dependabot.yml    # Dependabot config
│   └── PULL_REQUEST_TEMPLATE.md
├── Assets/
│   ├── Animations/       # Animation files
│   ├── Editor/           # Editor scripts
│   ├── Models/           # 3D models
│   ├── Prefabs/          # Unity prefabs
│   ├── Scenes/           # Unity scenes
│   └── Scripts/          # C# scripts
├── Packages/             # Unity packages
├── ProjectSettings/      # Unity project settings
├── CHANGELOG.md          # Changelog do template
├── CLAUDE.md             # Documentação para AI
├── CONTRIBUTING.md       # Este arquivo
└── ReadMe.md             # Documentação principal
```

### Onde Adicionar Novos Recursos

- **Workflows**: `.github/workflows/`
- **Scripts Unity**: `Assets/Scripts/` (organize em subpastas)
- **Documentação**: `*.md` files na raiz ou seções em `CLAUDE.md`
- **Configurações**: `ProjectSettings/` ou `Assets/Config File/`

---

## Perguntas Frequentes

### Posso usar este template em projetos comerciais?

Sim! Este template é para uso interno da Bugaboo Studio e projetos relacionados.

### Como mantenho meu fork atualizado?

```bash
git fetch upstream
git checkout develop
git merge upstream/develop
git push origin develop
```

### Devo fazer PR para main ou develop?

- **Features**: Sempre para `develop`
- **Hotfixes**: Para `main` (e depois merge para develop)
- **Documentação**: Preferível para `develop`

### Como testo minhas mudanças em workflows?

1. Fork o repositório
2. Faça mudanças no seu fork
3. Teste workflows no seu fork
4. Quando estável, abra PR

---

## Recursos Adicionais

- [CLAUDE.md](CLAUDE.md) - Documentação completa do template
- [CHANGELOG.md](CHANGELOG.md) - Histórico de mudanças
- [GameCI Documentation](https://game.ci/docs)
- [Unity Best Practices](https://unity.com/how-to/programming-unity)

---

## Contato

Dúvidas sobre contribuições?

- **Issues**: [GitHub Issues](https://github.com/bugaboostudio/ci_workflows_bugaboo/issues)
- **Discussions**: [GitHub Discussions](https://github.com/bugaboostudio/ci_workflows_bugaboo/discussions)
- **Email**: Contate Tiago (Bugaboo Studio) para questões de CI/CD

---

**Obrigado por contribuir! 🙏**

Cada contribuição, não importa o tamanho, ajuda a tornar este template melhor para toda a equipe.
