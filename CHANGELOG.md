# Changelog

Todas as mudanças notáveis neste template serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Planejado
- Workflow automático para deploy Android
- Workflow automático para deploy iOS
- Integration com Unity Cloud Build
- Automated changelog generation
- Pre-commit hooks para code quality

## [1.0.0] - 2026-01-05

### Adicionado
- **CLAUDE.md**: Documentação completa para assistentes de IA e desenvolvedores
  - GitFlow workflow completo com exemplos práticos
  - CI/CD best practices para Unity
  - Guia de uso do template com checklist
  - Conventional commits e versionamento semântico
  - Templates de arquivos e troubleshooting
- **CHANGELOG.md**: Arquivo de changelog para rastrear versões
- **CONTRIBUTING.md**: Guidelines de contribuição
- **.github/PULL_REQUEST_TEMPLATE.md**: Template padronizado de Pull Request
- **.github/dependabot.yml**: Configuração para auto-update de GitHub Actions
- **.github/CODEOWNERS**: Definição de code owners por diretório
- **ReadMe.md**: Atualizado com instruções de uso como template

### CI/CD Workflows
- `activation.yml`: Gerar arquivo de ativação da Unity license
- `main.yml`: Pipeline completo (test + build + deploy)
  - Tests em EditMode e PlayMode
  - Build para 6 plataformas (Windows, macOS, Linux, WebGL, iOS, Android)
  - Deploy automático WebGL para GitHub Pages
- `InspectCodeReSharper.yml`: Code inspection com ReSharper
- `CreateUnityPackage.yml`: Criação de Unity packages
- `SetupUnityEditor.yml`: Setup do Unity Editor
- `awscredencials.yml`: Configuração AWS
- `mainaws.yml`: Deploy para AWS

### Estrutura
- Estrutura de diretórios padrão para projetos Unity
- Editor script para criar estrutura automaticamente
- Integration com Ready Player Me SDK (v1.3.3)
- Suporte a animações de avatar
- Configurações de quality (Low/Medium/High)

### Configuração
- `.gitignore`: Unity-specific ignore patterns
- `.vscode/settings.json`: VSCode file exclusions
- `Packages/manifest.json`: Unity packages dependencies
- Avatar config files para diferentes quality levels

## [0.1.0] - 2023-06-30

### Adicionado (Versão Inicial)
- Configuração inicial do repositório Unity
- GitHub Actions workflows básicos
- Estrutura de projeto Unity
- Integração com GameCI
- Ready Player Me SDK integration
- Animações básicas de avatar
- Documentação inicial em ReadMe.md

---

## Template Version Guidelines

### Versionamento

Este template segue **Semantic Versioning**:

- **MAJOR** (X.0.0): Mudanças breaking que requerem ações dos usuários do template
  - Exemplo: Mudança de estrutura de diretórios, remoção de workflows
- **MINOR** (0.X.0): Novas features backward-compatible
  - Exemplo: Novos workflows, melhorias em documentação
- **PATCH** (0.0.X): Bugfixes e pequenas melhorias
  - Exemplo: Correções em workflows, typos em documentação

### Categorias de Mudanças

- **Adicionado**: Novas features
- **Modificado**: Mudanças em funcionalidades existentes
- **Depreciado**: Features que serão removidas em versões futuras
- **Removido**: Features removidas
- **Corrigido**: Bugfixes
- **Segurança**: Vulnerabilidades corrigidas

---

## Como Usar Este Changelog

### Para Mantenedores do Template

Ao fazer mudanças no template:

1. Adicione entrada na seção `[Unreleased]`
2. Use categoria apropriada (Adicionado, Modificado, etc.)
3. Descreva a mudança de forma clara
4. Quando fizer release, mova de `[Unreleased]` para nova versão

Exemplo:
```markdown
## [Unreleased]

### Adicionado
- Novo workflow para deploy em Steam

## [1.1.0] - 2026-02-01

### Adicionado
- Workflow para automated testing com coverage
```

### Para Usuários do Template

Ao atualizar template em projeto existente:

1. Compare versão atual com nova versão no changelog
2. Leia mudanças em cada versão intermediária
3. Preste atenção especial em "Removido" e "Depreciado"
4. Aplique mudanças manualmente conforme necessário

---

## Links Úteis

- [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/)
- [Semantic Versioning](https://semver.org/lang/pt-BR/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Nota**: Este changelog rastreia mudanças no **template**, não em projetos específicos criados a partir dele. Cada projeto deve manter seu próprio CHANGELOG.md.
