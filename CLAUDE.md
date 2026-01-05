# CLAUDE.md - AI Assistant Development Guide

## Repository Overview

This is a **Unity 3D project** managed by **Bugaboo Studio** that implements CI/CD workflows using **GitHub Actions** and **GameCI**. The project focuses on avatar-based 3D experiences using Ready Player Me integration.

**Repository**: `bugaboostudio/ci_workflows_bugaboo`
**Primary Language**: C# (Unity)
**CI/CD**: GitHub Actions with GameCI
**Main Branch**: Not specified in current configuration
**Current Active Branch**: `claude/add-claude-documentation-3M1sJ`

---

## Project Structure

### Root Directory Layout

```
/
├── .github/workflows/       # GitHub Actions CI/CD workflows
├── .vscode/                 # VSCode editor configuration
├── Assets/                  # Unity project assets (main development folder)
│   ├── Animations/          # Animation files and controllers
│   │   └── Avatar/          # Avatar-specific animations
│   ├── Audio/               # Audio assets
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── Voice/
│   ├── Config File/         # Avatar configuration files (Low/Medium/High)
│   ├── Editor/              # Unity Editor scripts
│   ├── Fonts/               # Font assets
│   ├── Materials/           # Material files
│   ├── Models/              # 3D models
│   │   ├── Avatars/
│   │   ├── Buildings/
│   │   ├── Furniture/
│   │   └── Miscellaneous/
│   ├── Plugins/             # Third-party plugins
│   ├── Prefabs/             # Unity prefabs
│   ├── Ready Player Me/     # Ready Player Me SDK files
│   ├── Samples/             # Sample scenes and scripts
│   ├── Scenes/              # Unity scenes
│   ├── Scripts/             # Custom C# scripts
│   └── UI/                  # UI elements
├── Packages/                # Unity Package Manager dependencies
├── ProjectSettings/         # Unity project configuration
└── ReadMe.md                # Project documentation (Portuguese)
```

### Key Files

- **`ReadMe.md`**: Portuguese documentation explaining GameCI setup
- **`Packages/manifest.json`**: Unity package dependencies
- **`.gitignore`**: Unity-specific ignore patterns from Bugaboo Studio
- **`.vscode/settings.json`**: VSCode file exclusions for Unity projects
- **`Assets/Editor/CreateDirectoryStructureMUNDOIEL.cs`**: Editor script for creating project directory structure

---

## Technology Stack

### Unity Packages

**Core Unity Modules**:
- Unity 2D Animation, Sprite, Tilemap
- Unity AI Navigation
- Cinemachine 2.9.5
- ProBuilder 5.0.7
- Post Processing 3.2.2
- TextMesh Pro 3.0.6
- Timeline 1.7.4

**Third-Party SDKs**:
- **Ready Player Me** (Avatar Loader, Core, WebView)
  - Avatar Loader: v1.3.3
  - WebView: v1.2.0
- **glTFast**: v5.0.0 (3D model loading)

**Development Tools**:
- Unity Test Framework 1.1.33
- JetBrains Rider 3.0.21
- Visual Studio Code 1.2.5
- Visual Studio 2.0.18
- Polybrush 1.1.4

### Development Environment

**Supported IDEs**:
- JetBrains Rider (recommended for C# development)
- Visual Studio
- Visual Studio Code

**Languages**:
- C# (primary scripting language)
- YAML (GitHub Actions workflows)

---

## CI/CD Workflows

All workflows are located in `.github/workflows/` and use **GameCI** actions.

### Available Workflows

#### 1. **Main Build Pipeline** (`main.yml`)
**Trigger**: Manual (`workflow_dispatch`)
**Purpose**: Complete test and build pipeline for all platforms

**Jobs**:
1. **checklicense**: Validates Unity license exists
2. **testRunner**: Runs tests in EditMode and PlayMode
3. **buildForAllSupportedPlatforms**: Builds for multiple platforms
   - StandaloneWindows64
   - StandaloneOSX
   - StandaloneLinux64
   - WebGL
   - iOS
   - Android
4. **deployPages**: Deploys WebGL build to GitHub Pages (branch: `gh-pages`)

**Caching Strategy**:
- Library cache per target platform
- LFS cache (commented out but available)

#### 2. **License Activation** (`activation.yml`)
**Trigger**: Manual (`workflow_dispatch`)
**Purpose**: Generate Unity license activation file
**Output**: Manual Activation File artifact for Unity licensing

#### 3. **Code Inspection** (`InspectCodeReSharper.yml`)
**Trigger**: Manual (`workflow_dispatch`)
**Purpose**: Run ReSharper code inspection
**Requirement**: Needs `MyProject.sln` solution file

#### 4. **Additional Workflows**
- `CreateUnityPackage.yml`: Package creation
- `SetupUnityEditor.yml`: Unity Editor setup
- `awscredencials.yml`: AWS credentials configuration
- `mainaws.yml`: AWS deployment pipeline

### Required Secrets

The following secrets must be configured in GitHub repository settings:

- **`UNITY_LICENSE`**: Unity license file content (required for all build workflows)
- AWS credentials (if using AWS workflows)

---

## Development Conventions

### Directory Structure Convention

The project follows a structured organization pattern defined in `Assets/Editor/CreateDirectoryStructureMUNDOIEL.cs`:

**Recommended Structure**:
```
Assets/
├── Scenes/
│   ├── Login/
│   ├── MainLobby/
│   ├── Auditoriums/
│   └── MeetingRooms/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController/
│   │   └── PlayerUI/
│   ├── NPC/
│   ├── InteractiveObjects/
│   ├── Networking/
│   │   ├── Photon/
│   │   └── Playfab/
│   ├── Managers/
│   └── Utils/
├── Prefabs/
│   ├── NPCs/
│   ├── Furniture/
│   └── InteractiveObjects/
└── UI/
    ├── MainMenu/
    ├── HUD/
    ├── Dialogues/
    └── Notifications/
```

### Code Style

**Language**: Primary documentation and comments are in **Portuguese (Brazilian)**

**Naming Conventions**:
- Follow Unity C# naming conventions
- Classes: PascalCase
- Methods: PascalCase
- Private fields: camelCase or _camelCase
- Public fields: PascalCase

### Git Conventions

**Commit Messages**: Use descriptive Portuguese messages following conventional commits
**Branch Naming**: Follow GitFlow conventions (see GitFlow section below)

---

## GitFlow Workflow (Template Best Practices)

Este repositório serve como **template** para novos projetos Unity. Siga o GitFlow para garantir organização e qualidade.

### Branch Structure

```
main (ou master)
├── develop
│   ├── feature/nome-da-feature
│   ├── feature/outra-feature
│   └── ...
├── release/v1.0.0
├── hotfix/correcao-critica
└── ...
```

### Branches Principais

#### **main** (ou **master**)
- **Propósito**: Código de produção estável
- **Proteção**: SEMPRE protegida, requer pull requests
- **Deployment**: Deploys automáticos para produção
- **Tags**: Todas as releases são taggeadas aqui (v1.0.0, v1.1.0, etc.)
- **Regra**: Nunca fazer commit direto na main

#### **develop**
- **Propósito**: Branch de integração para desenvolvimento
- **Proteção**: Requer pull requests
- **Estado**: Sempre deve estar em estado "buildável"
- **Origem**: Criada a partir da main
- **Destino**: Merge para main através de release branches

### Branches de Suporte

#### **feature/***
- **Nomenclatura**: `feature/nome-descritivo` ou `feature/ISSUE-123-descricao`
- **Origem**: Sempre criada a partir de `develop`
- **Destino**: Merge de volta para `develop`
- **Ciclo de Vida**: Excluída após merge
- **Exemplo**: `feature/avatar-customization`, `feature/multiplayer-lobby`

**Workflow**:
```bash
# Criar feature branch
git checkout develop
git pull origin develop
git checkout -b feature/nova-funcionalidade

# Trabalhar na feature...
git add .
git commit -m "feat: adiciona sistema de inventário"

# Finalizar feature
git checkout develop
git pull origin develop
git merge feature/nova-funcionalidade
git push origin develop
git branch -d feature/nova-funcionalidade
```

#### **release/***
- **Nomenclatura**: `release/v1.2.0` (usar versionamento semântico)
- **Origem**: Criada a partir de `develop`
- **Destino**: Merge para `main` E `develop`
- **Propósito**: Preparação final para release (bugfixes, ajustes de versão)
- **Regra**: Nenhuma nova feature, apenas bugfixes

**Workflow**:
```bash
# Criar release branch
git checkout develop
git checkout -b release/v1.2.0

# Ajustar versão no ProjectSettings
# Fazer bugfixes finais...
git commit -am "chore: bump version to 1.2.0"

# Finalizar release
git checkout main
git merge release/v1.2.0
git tag -a v1.2.0 -m "Release version 1.2.0"
git push origin main --tags

# Merge de volta para develop
git checkout develop
git merge release/v1.2.0
git push origin develop

# Excluir branch
git branch -d release/v1.2.0
```

#### **hotfix/***
- **Nomenclatura**: `hotfix/correcao-critica` ou `hotfix/v1.2.1`
- **Origem**: Criada a partir de `main`
- **Destino**: Merge para `main` E `develop`
- **Propósito**: Correções urgentes em produção
- **Ciclo de Vida**: Excluída após merge

**Workflow**:
```bash
# Criar hotfix
git checkout main
git checkout -b hotfix/v1.2.1

# Fazer correção...
git commit -am "fix: corrige crash ao carregar avatar"

# Finalizar hotfix
git checkout main
git merge hotfix/v1.2.1
git tag -a v1.2.1 -m "Hotfix version 1.2.1"
git push origin main --tags

# Merge para develop também
git checkout develop
git merge hotfix/v1.2.1
git push origin develop

# Excluir branch
git branch -d hotfix/v1.2.1
```

### Conventional Commits

Use o padrão de conventional commits para mensagens claras:

```
tipo(escopo): descrição curta

[corpo opcional]

[rodapé opcional]
```

**Tipos**:
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Alterações em documentação
- `style`: Formatação, ponto e vírgula faltando, etc.
- `refactor`: Refatoração de código
- `test`: Adição ou correção de testes
- `chore`: Tarefas de manutenção, atualizações de dependências
- `perf`: Melhorias de performance
- `ci`: Alterações em CI/CD

**Exemplos**:
```
feat(avatar): adiciona sistema de customização de roupas
fix(networking): corrige desconexão em salas privadas
docs(readme): atualiza instruções de instalação
chore(deps): atualiza Ready Player Me SDK para v1.4.0
ci(github): adiciona workflow de code review automático
```

### Pull Request Workflow

1. **Criar PR** a partir da feature branch para develop
2. **Template de PR** (criar em `.github/PULL_REQUEST_TEMPLATE.md`):
   ```markdown
   ## Descrição
   [Descreva as mudanças]

   ## Tipo de Mudança
   - [ ] Nova feature
   - [ ] Bugfix
   - [ ] Refatoração
   - [ ] Documentação

   ## Checklist
   - [ ] Código segue o style guide do projeto
   - [ ] Testes foram adicionados/atualizados
   - [ ] Build local passou sem erros
   - [ ] Documentação foi atualizada
   - [ ] Sem warnings no console Unity
   ```

3. **Code Review**: Pelo menos 1 aprovação necessária
4. **CI deve passar**: Todos os workflows devem estar verdes
5. **Squash ou Merge**: Escolher estratégia consistente

### Versionamento Semântico

Usar [Semantic Versioning 2.0.0](https://semver.org/):

```
MAJOR.MINOR.PATCH (ex: 1.2.3)
```

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Nova funcionalidade (backward compatible)
- **PATCH**: Correção de bugs (backward compatible)

**Exemplos**:
- `v0.1.0`: Desenvolvimento inicial
- `v1.0.0`: Primeira release de produção
- `v1.1.0`: Adicionou multiplayer
- `v1.1.1`: Corrigiu bug de conexão
- `v2.0.0`: Refatoração completa da arquitetura de rede

### Protected Branches

Configure no GitHub:

**main**:
- ✅ Require pull request before merging
- ✅ Require approvals (1+)
- ✅ Require status checks to pass
- ✅ Require conversation resolution
- ✅ Do not allow bypassing

**develop**:
- ✅ Require pull request before merging
- ✅ Require status checks to pass

---

## CI/CD Best Practices para Unity

### Workflow Optimization

#### 1. **Library Caching Strategy**

A pasta `Library/` da Unity é grande e cara para reconstruir. Use cache agressivo:

```yaml
- name: Cache Unity Library
  uses: actions/cache@v3
  with:
    path: Library
    key: Library-${{ matrix.targetPlatform }}-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
    restore-keys: |
      Library-${{ matrix.targetPlatform }}-
      Library-
```

**Benefícios**:
- Reduz tempo de build de ~15 minutos para ~5 minutos
- Economiza compute time do GitHub Actions

#### 2. **Matrix Strategy para Builds Paralelos**

```yaml
strategy:
  fail-fast: false  # Continue outros builds mesmo se um falhar
  matrix:
    targetPlatform:
      - StandaloneWindows64
      - Android
      - iOS
      - WebGL
```

**Benefícios**:
- Builds paralelos = tempo total reduzido
- `fail-fast: false` = vê todos os erros de uma vez

#### 3. **Separação de Workflows**

**Recomendação**: Separar em múltiplos workflows para eficiência

```
.github/workflows/
├── test.yml              # Roda em todos os PRs
├── build-dev.yml         # Build de desenvolvimento (develop)
├── build-release.yml     # Build de release (main)
├── deploy-staging.yml    # Deploy para staging
├── deploy-production.yml # Deploy para produção
└── manual-build.yml      # Build manual sob demanda
```

**Exemplo - test.yml** (otimizado para PRs):
```yaml
name: Test Unity Project

on:
  pull_request:
    branches: [develop, main]
  push:
    branches: [develop]

jobs:
  test:
    name: Test in ${{ matrix.testMode }}
    runs-on: ubuntu-latest
    strategy:
      matrix:
        testMode:
          - EditMode
          - PlayMode
    steps:
      - uses: actions/checkout@v3
        with:
          lfs: true

      - uses: actions/cache@v3
        with:
          path: Library
          key: Library-test-${{ hashFiles('Assets/**', 'Packages/**') }}
          restore-keys: Library-test-

      - uses: game-ci/unity-test-runner@v2
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          testMode: ${{ matrix.testMode }}
          checkName: ${{ matrix.testMode }} Test Results

      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: Test results (${{ matrix.testMode }})
          path: artifacts/
```

#### 4. **Build Seletivo por Plataforma**

Nem sempre precisa buildar todas as plataformas:

```yaml
on:
  workflow_dispatch:
    inputs:
      platforms:
        description: 'Platforms to build (comma-separated)'
        required: true
        default: 'StandaloneWindows64,Android'
        type: choice
        options:
          - All
          - StandaloneWindows64
          - Android
          - iOS
          - WebGL
```

#### 5. **Versionamento Automático**

Use GitVersion ou script customizado:

```yaml
- name: Generate Version
  id: version
  run: |
    VERSION=$(git describe --tags --always)
    echo "version=$VERSION" >> $GITHUB_OUTPUT
    echo "Version: $VERSION"

- name: Update Unity Version
  run: |
    # Atualizar ProjectSettings/ProjectSettings.asset
    sed -i "s/bundleVersion: .*/bundleVersion: ${{ steps.version.outputs.version }}/" ProjectSettings/ProjectSettings.asset
```

#### 6. **Artifact Management**

```yaml
- uses: actions/upload-artifact@v3
  with:
    name: Build-${{ matrix.targetPlatform }}-${{ github.sha }}
    path: build/${{ matrix.targetPlatform }}
    retention-days: 7  # Limpar artifacts antigos
```

**Retention Policy**:
- **Development builds**: 7 dias
- **Release builds**: 90 dias
- **Nightly builds**: 3 dias

#### 7. **Testing Strategy**

**3 Níveis de Testes**:

1. **Unit Tests** (EditMode): Testes rápidos de lógica
2. **Integration Tests** (PlayMode): Testes de sistemas integrados
3. **Smoke Tests**: Build e execução básica

```yaml
# Adicionar smoke test
- name: Smoke Test Build
  run: |
    # Para Linux build, testar execução headless
    xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
      ./build/StandaloneLinux64/MyGame.x86_64 -batchmode -nographics -quit
```

#### 8. **LFS (Large File Storage) Optimization**

```yaml
- name: Create LFS file list
  run: git lfs ls-files -l | cut -d' ' -f1 | sort > .lfs-assets-id

- name: Restore LFS cache
  uses: actions/cache@v3
  with:
    path: .git/lfs
    key: lfs-${{ hashFiles('.lfs-assets-id') }}

- name: Git LFS Pull
  run: git lfs pull
```

**Quando usar LFS**:
- Assets grandes (> 1MB): modelos 3D, texturas, áudio
- Assets binários que mudam frequentemente
- **Não use para**: Code, pequenos configs, textos

#### 9. **Security Best Practices**

```yaml
# Nunca expor secrets em logs
- name: Build
  env:
    UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
    # NÃO faça echo de secrets!
  run: |
    echo "Building project..." # OK
    # echo "$UNITY_LICENSE" # NUNCA!
```

**Secrets necessários**:
- `UNITY_LICENSE`: Conteúdo do arquivo .ulf
- `UNITY_EMAIL`: Email da conta Unity (para CI/CD)
- `UNITY_PASSWORD`: Senha da conta Unity (para CI/CD)
- Platform-specific: Android keystore, iOS certificates, etc.

#### 10. **Conditional Workflows**

```yaml
# Só rodar build se testes passarem
deploy:
  needs: [test, build]
  if: github.ref == 'refs/heads/main'
  runs-on: ubuntu-latest
  # ...
```

**Condições úteis**:
- `if: github.event_name == 'pull_request'`
- `if: startsWith(github.ref, 'refs/tags/v')`
- `if: contains(github.event.head_commit.message, '[skip ci]')`

### Performance Benchmarks

**Tempos esperados** (com cache):
- **Tests (EditMode + PlayMode)**: 3-5 minutos
- **Build Windows/Linux**: 10-15 minutos
- **Build Android**: 15-20 minutos
- **Build iOS**: 20-25 minutos
- **Build WebGL**: 15-20 minutos

**Sem cache**: 2-3x mais lento

### Cost Optimization (GitHub Actions)

**Free tier**: 2,000 minutos/mês (Linux)

**Estratégias de economia**:
1. Cache agressivo (salva ~70% do tempo)
2. Builds seletivos (não buildar tudo sempre)
3. Tests em PRs, builds só em merges
4. Self-hosted runners para projetos grandes

---

## Usando Este Repositório Como Template

Este repositório foi criado para servir como **template inicial** para novos projetos Unity na Bugaboo Studio.

### Checklist de Configuração Inicial

Ao criar um novo projeto a partir deste template:

#### 1. **Configuração do Repositório**

- [ ] Criar novo repositório a partir deste template no GitHub
- [ ] Clonar repositório localmente
- [ ] Renomear projeto Unity em `ProjectSettings/ProjectSettings.asset`
- [ ] Atualizar `ReadMe.md` com informações do novo projeto
- [ ] Atualizar este `CLAUDE.md` com especificidades do projeto

#### 2. **Configuração do Git**

- [ ] Criar branch `develop` a partir de `main`
  ```bash
  git checkout -b develop
  git push -u origin develop
  ```
- [ ] Configurar protected branches no GitHub:
  - Proteger `main` (require PR, require approvals)
  - Proteger `develop` (require PR, require status checks)
- [ ] Configurar default branch para `develop` nas settings do GitHub

#### 3. **Configuração de CI/CD**

- [ ] Gerar Unity license:
  ```bash
  # Disparar workflow de activation
  gh workflow run activation.yml
  ```
- [ ] Baixar Manual Activation File do workflow
- [ ] Fazer upload em https://license.unity3d.com/manual
- [ ] Baixar arquivo `.ulf`
- [ ] Adicionar secret `UNITY_LICENSE` no GitHub:
  - Settings > Secrets > New repository secret
  - Nome: `UNITY_LICENSE`
  - Valor: Conteúdo completo do arquivo `.ulf`

#### 4. **Configuração de Plataformas**

Dependendo das plataformas alvo do projeto, configurar secrets adicionais:

**Android**:
- [ ] Criar keystore para assinatura
- [ ] Adicionar secrets:
  - `ANDROID_KEYSTORE_BASE64`: Keystore codificado em base64
  - `ANDROID_KEYSTORE_PASS`: Senha do keystore
  - `ANDROID_KEYALIAS_NAME`: Nome do alias
  - `ANDROID_KEYALIAS_PASS`: Senha do alias

**iOS**:
- [ ] Configurar provisioning profiles
- [ ] Adicionar certificates no GitHub secrets

**WebGL**:
- [ ] Habilitar GitHub Pages nas settings
- [ ] Configurar para usar branch `gh-pages`
- [ ] (Opcional) Configurar custom domain

#### 5. **Estrutura de Diretórios**

- [ ] Abrir projeto na Unity
- [ ] Executar `Assets > Create > Directory Structure` para criar estrutura padrão
- [ ] Revisar e personalizar estrutura conforme necessidade do projeto
- [ ] Fazer commit inicial da estrutura:
  ```bash
  git add .
  git commit -m "chore: setup initial directory structure"
  git push origin develop
  ```

#### 6. **Configuração de Packages**

- [ ] Revisar `Packages/manifest.json`
- [ ] Remover packages não necessários (ex: Ready Player Me se não for usar)
- [ ] Adicionar packages específicos do projeto
- [ ] Atualizar versions para as mais recentes (se desejado)

#### 7. **Configuração de Workflows**

Personalizar workflows em `.github/workflows/`:

- [ ] **test.yml**: Ajustar triggers conforme necessidade
- [ ] **main.yml**: Remover plataformas não necessárias da matrix
- [ ] **build-dev.yml**: Criar se precisar builds automáticos no develop
- [ ] **deploy-production.yml**: Configurar deployment específico do projeto

#### 8. **Documentação**

- [ ] Criar `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Criar `.github/ISSUE_TEMPLATE/` com templates para:
  - Bug reports
  - Feature requests
  - Documentation improvements
- [ ] Atualizar `ReadMe.md` com:
  - Nome do projeto
  - Descrição
  - Como rodar localmente
  - Links importantes
- [ ] Adicionar `CONTRIBUTING.md` se projeto open source

#### 9. **Configurações Adicionais**

- [ ] Configurar Code Owners (`.github/CODEOWNERS`)
- [ ] Configurar Dependabot para atualizar actions (`.github/dependabot.yml`)
- [ ] Adicionar badges no ReadMe.md (build status, tests, etc.)
- [ ] Configurar notifications de CI/CD (Slack, Discord, etc.)

#### 10. **Primeiro Release**

- [ ] Criar primeira release v0.1.0
  ```bash
  git checkout develop
  git checkout -b release/v0.1.0
  # Ajustar versão no Unity
  git commit -am "chore: bump version to 0.1.0"
  git checkout main
  git merge release/v0.1.0
  git tag -a v0.1.0 -m "Initial release"
  git push origin main --tags
  git checkout develop
  git merge release/v0.1.0
  git push origin develop
  ```

### Personalizações Recomendadas

#### Remover Ready Player Me (se não usar)

Se o projeto não usa avatares do Ready Player Me:

```bash
# Remover packages
# Editar Packages/manifest.json e remover:
# - com.readyplayerme.avatarloader
# - com.readyplayerme.core
# - com.readyplayerme.webview

# Remover samples
rm -rf "Assets/Samples/Ready Player Me Avatar Loader"
rm -rf "Assets/Samples/Ready Player Me WebView"
rm -rf "Assets/Ready Player Me"

# Commit
git add .
git commit -m "chore: remove Ready Player Me integration"
```

#### Adicionar Networking (Photon/PlayFab)

Se o projeto usa multiplayer:

```bash
# Criar estrutura de networking
mkdir -p Assets/Scripts/Networking/Photon
mkdir -p Assets/Scripts/Networking/PlayFab

# Adicionar packages via Package Manager UI ou manifest.json
```

#### Configurar Quality Settings

Ajustar quality settings para cada plataforma:

1. Abrir `Edit > Project Settings > Quality`
2. Configurar quality levels para:
   - Mobile (Low/Medium)
   - Desktop (High/Ultra)
   - WebGL (Medium)
3. Salvar e commitar:
   ```bash
   git add ProjectSettings/QualitySettings.asset
   git commit -m "chore: configure quality settings per platform"
   ```

### Template Maintenance

#### Mantendo o Template Atualizado

Se você melhorar este template e quiser propagar mudanças para projetos existentes:

1. **Fazer mudanças no template**:
   ```bash
   # No repositório template
   git checkout main
   # Fazer melhorias...
   git commit -m "chore: improve CI/CD caching strategy"
   git push origin main
   git tag -a template-v1.1.0 -m "Template version 1.1.0"
   git push --tags
   ```

2. **Aplicar em projetos existentes**:
   ```bash
   # No projeto que usa o template
   git remote add template https://github.com/bugaboostudio/ci_workflows_bugaboo.git
   git fetch template
   git checkout -b update-from-template
   git merge template/main --allow-unrelated-histories
   # Resolver conflitos...
   git commit -m "chore: update from template v1.1.0"
   ```

### Best Practices para Templates

1. **Mantenha Genérico**: Template deve ser útil para maioria dos projetos
2. **Documente Bem**: Explique cada parte do template
3. **Versione o Template**: Use tags para versões do template
4. **Changelog**: Mantenha CHANGELOG.md com mudanças no template
5. **Minimal by Default**: Inclua apenas o essencial, documente como adicionar extras

### Estrutura Mínima vs Completa

**Template Mínimo** (este repositório):
- Estrutura básica de diretórios
- CI/CD essencial (test + build)
- GitIgnore configurado
- VSCode settings básico

**Adicionar Conforme Necessário**:
- SDKs específicos (Ready Player Me, Photon, etc.)
- Networking infrastructure
- Authentication systems
- Analytics integration
- Monetization (IAP, Ads)

### Exemplo de Novo Projeto

```bash
# 1. Criar repositório a partir do template
gh repo create meu-novo-jogo --template bugaboostudio/ci_workflows_bugaboo --private

# 2. Clonar
git clone https://github.com/bugaboostudio/meu-novo-jogo.git
cd meu-novo-jogo

# 3. Criar branch develop
git checkout -b develop
git push -u origin develop

# 4. Configurar Unity License (via GitHub Actions)

# 5. Abrir na Unity e configurar

# 6. Primeiro commit
git add .
git commit -m "chore: initial project setup from template"
git push origin develop

# 7. Criar primeira feature
git checkout -b feature/player-movement
# ... desenvolver ...
git commit -am "feat: add basic player movement"
git push -u origin feature/player-movement
# ... criar PR no GitHub ...
```

---

## Important File Locations

### Configuration Files

- **Avatar Quality Configs**: `Assets/Config File/`
  - `Avatar ConfigLow.asset`
  - `Avatar ConfigMedium.asset`
  - `Avatar ConfigHigh.asset`

### Animation Assets

- **Avatar Animations**: `Assets/Animations/Avatar/`
  - Animation Pack FBX file
  - Multiple animation controllers
  - Idle variations (Crossing Arms, Disapproval, Watch, Clapping, ThumbsUp)
  - Sitting variations
  - Walking animations

### Scenes

- **Main Scene**: `Assets/Scenes/Main.unity`
- **Sample Scenes**: `Assets/Samples/Ready Player Me/` (various example scenes)

### Scripts

- **Editor Scripts**: `Assets/Editor/`
- **Sample Scripts**: `Assets/Samples/Ready Player Me Avatar Loader/1.3.3/QuickStart/Scripts/`
  - Player input and movement
  - Camera controls
  - Avatar loading

---

## Git Workflow

### Branch Strategy

**Current Branch**: `claude/add-claude-documentation-3M1sJ`

### Pushing Changes

Always use:
```bash
git push -u origin <branch-name>
```

**Important**: Branch names should start with `claude/` and end with matching session ID to avoid 403 errors.

### Network Retry Policy

For git operations (push/fetch/pull), retry up to 4 times with exponential backoff:
- 2s, 4s, 8s, 16s intervals

### Recent Commits

Latest activity focused on:
- ReadMe.md updates
- 3D Animations integration
- Ready Player Me SDK integration
- CI workflow improvements

---

## Testing Strategy

### Test Modes

The project uses Unity Test Framework with two test modes:

1. **EditMode Tests**: Tests that run in the Unity Editor
2. **PlayMode Tests**: Tests that run in Play mode

### Running Tests

**Locally**: Use Unity Test Runner window
**CI/CD**: Automatic execution on workflow dispatch via `main.yml`

Test results are uploaded as artifacts in GitHub Actions.

---

## Build Process

### Build Targets

The project supports multiple build targets:
- **Windows**: StandaloneWindows64
- **macOS**: StandaloneOSX
- **Linux**: StandaloneLinux64
- **Web**: WebGL (deployed to GitHub Pages)
- **Mobile**: iOS, Android

### Build Configuration

**Build Jobs**: Each platform builds independently with fail-fast disabled
**Artifacts**: Build outputs are uploaded as GitHub Actions artifacts
**Deployment**: WebGL builds automatically deploy to `gh-pages` branch

### Building Locally

Use Unity Editor Build Settings to create builds for desired platforms.

---

## Common Tasks and Commands

### Create Directory Structure

Use Unity menu: `Assets > Create > Directory Structure`

This creates the standard project folder hierarchy.

### Running CI Workflows

All workflows are manual dispatch:

```bash
# Trigger via GitHub UI or CLI
gh workflow run main.yml
gh workflow run activation.yml
gh workflow run InspectCodeReSharper.yml
```

### Unity Package Management

Edit `Packages/manifest.json` to add/remove packages.

**Key Dependencies**:
- Ready Player Me packages (from GitHub repositories)
- glTFast for glTF/GLB model loading

---

## Working with AI Assistants

### When Making Changes

1. **Read Before Modifying**: Always read files before suggesting changes
2. **Preserve Portuguese**: Maintain Portuguese comments and documentation
3. **Follow Structure**: Use the predefined directory structure
4. **Test Locally**: Consider Unity Editor compatibility
5. **Check CI**: Ensure changes don't break GitHub Actions workflows

### File Exclusions

VSCode is configured to hide Unity-generated files. Don't modify:
- `.meta` files (unless necessary)
- `Library/` folder
- `Temp/` folder
- `Builds/` folder
- Binary assets (.fbx, .png, .jpg, etc.) without explicit request

### Unity-Specific Considerations

- **Meta Files**: Unity generates `.meta` files for asset tracking - generally don't edit these
- **Scene Files**: `.unity` scene files are YAML-based but complex - use Unity Editor for modifications
- **Prefabs**: Similar to scenes, prefer Unity Editor for prefab changes
- **Asset Serialization**: Unity uses custom serialization - respect existing formats

### Code Quality

- ReSharper inspection is available via workflow
- Follow Unity best practices for MonoBehaviour lifecycle
- Consider performance implications (mobile targets included)

---

## Ready Player Me Integration

### SDK Structure

The project uses Ready Player Me for avatar creation and loading:

**Main Components**:
- Avatar Loader SDK (v1.3.3)
- Core SDK
- WebView SDK (v1.2.0)

**Sample Implementations**:
- Quick Start: Basic avatar loading with third-person controller
- Avatar LOD: Level of detail management
- Multiple Avatar Loading/Rendering
- Quality-based loading (Low/Medium/High configs)

### Avatar Configuration

Three quality presets available:
- `Avatar ConfigLow.asset`
- `Avatar ConfigMedium.asset`
- `Avatar ConfigHigh.asset`

Use these for performance optimization across platforms.

---

## Troubleshooting

### Common Issues

**Unity License Errors**:
- Ensure `UNITY_LICENSE` secret is properly set
- Run `activation.yml` workflow to generate new license file

**Build Failures**:
- Check Library cache validity
- Verify all required packages are installed
- Review error logs in GitHub Actions artifacts

**Git Push 403 Errors**:
- Ensure branch name starts with `claude/`
- Verify branch name ends with session ID

**Missing Dependencies**:
- Run `Unity Package Manager > Resolve` in Unity Editor
- Check `Packages/manifest.json` for Git URL availability

---

## Additional Resources

### Official Documentation

- **GameCI Documentation**: https://game.ci/docs/github/getting-started
- **Unity Actions GitHub**: https://github.com/game-ci/unity-actions
- **Ready Player Me Unity SDK**: Check package repositories for documentation

### Project-Specific Notes

- Primary language: Portuguese (Brazilian)
- Company: Bugaboo Studio
- Contact: Tiago (mentioned in ReadMe.md) for CI/CD configuration help

---

## Version History

**Last Updated**: 2026-01-05
**Document Version**: 2.0
**Repository State**: Template repository for Unity projects with GitFlow and CI/CD best practices

**Changelog**:
- **v2.0** (2026-01-05):
  - Added comprehensive GitFlow workflow documentation
  - Added CI/CD best practices for Unity
  - Added template usage guide with checklist
  - Added conventional commits guidelines
  - Added versioning strategies
  - Added template maintenance section
- **v1.0** (2026-01-05): Initial documentation created

---

## Notes for AI Assistants

### Critical Guidelines

1. **Always read files before modifying** - Unity projects have complex interdependencies
2. **Respect Portuguese language** - Documentation and comments are in Portuguese
3. **Use GameCI patterns** - Follow existing workflow patterns for CI/CD changes
4. **Test considerations** - Changes should work across multiple platforms (Windows, Mac, Linux, iOS, Android, WebGL)
5. **Asset handling** - Be cautious with binary assets; prefer Unity Editor for asset modifications
6. **Branch naming** - Follow `claude/` prefix convention for automated workflows
7. **Commit practices** - Write clear, descriptive commits in Portuguese when appropriate
8. **CI/CD awareness** - All builds run through GitHub Actions; local testing may not catch CI-specific issues

### Best Practices

- **Before suggesting structural changes**, review `CreateDirectoryStructureMUNDOIEL.cs` for intended structure
- **When adding scripts**, place them in appropriate subdirectories under `Assets/Scripts/`
- **For networking features**, note the planned Photon/PlayFab integration structure
- **Mobile considerations**: Project targets iOS and Android - consider touch input, performance, and platform-specific requirements
- **WebGL deployment**: Remember that WebGL builds deploy to GitHub Pages automatically

### Common Pitfalls to Avoid

- Don't commit files in `.gitignore` (Library, Temp, Build folders, etc.)
- Don't modify Unity meta files unless absolutely necessary
- Don't break GameCI workflow compatibility
- Don't ignore platform-specific requirements (6 platforms supported)
- Don't assume Windows-only environment (cross-platform project)

---

## Recursos e Referências Úteis

### Documentação Oficial

**GameCI**:
- [GameCI Documentation](https://game.ci/docs)
- [Unity Test Runner](https://game.ci/docs/github/test-runner)
- [Unity Builder](https://game.ci/docs/github/builder)
- [Activation Guide](https://game.ci/docs/github/activation)
- [GameCI GitHub](https://github.com/game-ci/unity-actions)

**Unity**:
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [Unity Best Practices](https://unity.com/how-to/programming-unity)

**Git & GitHub**:
- [GitFlow Original](https://nvie.com/posts/a-successful-git-branching-model/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)

### Ferramentas Recomendadas

**Development**:
- [JetBrains Rider](https://www.jetbrains.com/rider/) - IDE recomendado para Unity
- [Visual Studio](https://visualstudio.microsoft.com/) - IDE alternativo
- [VSCode](https://code.visualstudio.com/) - Editor leve

**Git Tools**:
- [GitHub CLI](https://cli.github.com/) - Comandos git via CLI
- [GitKraken](https://www.gitkraken.com/) - Git GUI (opcional)
- [SourceTree](https://www.sourcetreeapp.com/) - Git GUI alternativo

**CI/CD**:
- [act](https://github.com/nektos/act) - Rodar GitHub Actions localmente
- [GitVersion](https://gitversion.net/) - Versionamento automático
- [semantic-release](https://github.com/semantic-release/semantic-release) - Automação de releases

**Unity Tools**:
- [Unity Hub](https://unity.com/download) - Gerenciador de versões Unity
- [Asset Store](https://assetstore.unity.com/) - Assets e ferramentas
- [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) - Gerenciador de pacotes

### Scripts Úteis

**Gerar keystore Android**:
```bash
keytool -genkey -v -keystore meu-jogo.keystore -alias meu-jogo -keyalg RSA -keysize 2048 -validity 10000
```

**Converter keystore para base64** (para GitHub secret):
```bash
base64 meu-jogo.keystore | tr -d '\n' > keystore-base64.txt
```

**Trigger workflow via CLI**:
```bash
gh workflow run main.yml
gh workflow view main.yml --web
gh run list --workflow=main.yml
```

**Listar artifacts de um run**:
```bash
gh run download <run-id>
```

**Atualizar todos os packages Unity**:
```bash
# Usar Unity Package Manager UI ou
# Editar Packages/manifest.json e remover versões fixas
```

### Templates de Arquivos

**`.github/PULL_REQUEST_TEMPLATE.md`**:
```markdown
## 📝 Descrição
<!-- Descreva as mudanças de forma clara e concisa -->

## 🎯 Tipo de Mudança
- [ ] 🚀 Nova feature (mudança não-breaking que adiciona funcionalidade)
- [ ] 🐛 Bugfix (mudança não-breaking que corrige um issue)
- [ ] 💥 Breaking change (fix ou feature que causa mudança em funcionalidade existente)
- [ ] 📚 Documentação
- [ ] 🎨 Refatoração

## ✅ Checklist
- [ ] Meu código segue o style guide do projeto
- [ ] Fiz self-review do meu código
- [ ] Comentei partes complexas do código
- [ ] Atualizei a documentação
- [ ] Minhas mudanças não geram novos warnings
- [ ] Adicionei testes que provam que meu fix funciona ou feature funciona
- [ ] Testes unitários novos e existentes passam localmente
- [ ] Build local passou sem erros
- [ ] Testei em todas as plataformas alvo (se aplicável)

## 🧪 Como Testar
<!-- Descreva os passos para testar as mudanças -->

1.
2.
3.

## 📸 Screenshots/Videos
<!-- Se aplicável, adicione screenshots ou videos das mudanças -->

## 🔗 Issues Relacionadas
<!-- Link para issues: Closes #123, Fixes #456 -->
```

**`.github/dependabot.yml`**:
```yaml
version: 2
updates:
  # Manter GitHub Actions atualizadas
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
    labels:
      - "dependencies"
      - "github-actions"
```

**`.github/CODEOWNERS`**:
```
# Default owners for everything in the repo
*       @bugaboostudio/developers

# Specific paths
/.github/workflows/    @bugaboostudio/devops
/Assets/Scripts/       @bugaboostudio/developers
/Packages/             @bugaboostudio/tech-leads
```

### Troubleshooting Common Issues

**Problema: Unity License expirou**
```bash
# Gerar nova license
gh workflow run activation.yml
# Baixar artifact, fazer upload, atualizar secret
```

**Problema: Build falha com "Library corrupted"**
```bash
# Limpar cache do GitHub Actions
# Vai em Actions > Caches > Delete all caches
# Ou usar GitHub CLI:
gh cache delete --all
```

**Problema: Git LFS bandwidth limit**
```bash
# Se atingir limite do LFS:
# 1. Considerar self-hosted storage
# 2. Ou comprar mais bandwidth
# 3. Ou otimizar assets (comprimir texturas, etc.)
```

**Problema: Android build falha na assinatura**
```bash
# Verificar que secrets estão corretos:
# - ANDROID_KEYSTORE_BASE64
# - ANDROID_KEYSTORE_PASS
# - ANDROID_KEYALIAS_NAME
# - ANDROID_KEYALIAS_PASS

# Regenerar keystore se necessário
```

**Problema: WebGL build muito grande**
```bash
# Otimizações:
# 1. Habilitar compression (Brotli)
# 2. Remover assets não usados
# 3. Usar Asset Bundles
# 4. Strip engine code
# 5. Reduce quality settings para WebGL
```

### Performance Tips

**Reduzir tempo de build**:
1. Use cache agressivo
2. Build apenas plataformas necessárias
3. Use incremental builds quando possível
4. Considere self-hosted runners (mais potentes)

**Reduzir uso de bandwidth LFS**:
1. Não commitar assets desnecessários
2. Comprimir texturas antes de commitar
3. Use .gitattributes para rastrear apenas assets grandes
4. Considere Asset Bundles para assets muito grandes

**Otimizar testes**:
1. Rode apenas testes afetados pelas mudanças (se possível)
2. Use test categories para rodar subsets
3. Paralelizar testes quando possível
4. Mock dependencies pesadas

### Comunidade e Suporte

**Bugaboo Studio**:
- Contact: Tiago (para questões de CI/CD)
- Internal wiki: [adicionar link se existir]
- Slack/Discord: [adicionar link se existir]

**Comunidades Unity**:
- [Unity Forum](https://forum.unity.com/)
- [r/Unity3D](https://www.reddit.com/r/Unity3D/)
- [Unity Discord](https://discord.com/invite/unity)

**GameCI Community**:
- [GameCI Discord](https://game.ci/discord)
- [GitHub Discussions](https://github.com/game-ci/unity-builder/discussions)

---

*Este documento é mantido para assistentes de IA e desenvolvedores trabalhando com projetos Unity da Bugaboo Studio. Atualize conforme o projeto evolui.*

**Próximos Passos Sugeridos**:
1. Criar `CHANGELOG.md` para rastrear mudanças do template
2. Criar `CONTRIBUTING.md` com guidelines de contribuição
3. Adicionar badges de status ao `ReadMe.md`
4. Configurar automated dependency updates
5. Criar workflows adicionais para deploy específico de cada plataforma
