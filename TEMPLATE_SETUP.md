# 🚀 Template Setup Guide - Bugaboo Studio Unity Template

Este documento guia você pelo processo de configuração de um novo projeto Unity criado a partir deste template.

---

## 📋 Pré-Requisitos

Antes de começar, certifique-se de ter:

- [ ] **Unity Hub** instalado (https://unity.com/download)
- [ ] **Unity 2022.3 LTS** ou superior instalado via Unity Hub
- [ ] **Git** configurado localmente
- [ ] **GitHub CLI** (opcional mas recomendado): `brew install gh` ou https://cli.github.com/
- [ ] **Conta GitHub** com acesso à organização Bugaboo Studio
- [ ] **Acesso à Unity License** (Personal, Plus, ou Pro)

---

## 🎯 Passo 1: Criar Repositório a Partir do Template

### Opção A: Usando GitHub Web Interface (Recomendado)

1. Acesse https://github.com/bugaboostudio/ci_workflows_bugaboo
2. Clique no botão verde **"Use this template"** → **"Create a new repository"**
3. Preencha:
   - **Owner**: `bugaboostudio`
   - **Repository name**: `nome-do-seu-jogo` (use kebab-case)
   - **Description**: Breve descrição do projeto
   - **Visibility**: Private (geralmente)
4. **✅ Include all branches**: Desmarque (só precisamos da main)
5. Clique em **"Create repository"**

### Opção B: Usando GitHub CLI

```bash
gh repo create bugaboostudio/nome-do-seu-jogo \
  --template bugaboostudio/ci_workflows_bugaboo \
  --private \
  --clone

cd nome-do-seu-jogo
```

---

## 🌿 Passo 2: Configurar GitFlow

### 2.1: Criar Branch `develop`

```bash
# Criar e publicar branch develop
git checkout -b develop
git push -u origin develop
```

### 2.2: Configurar `develop` como Branch Padrão

**Via GitHub CLI:**
```bash
gh repo edit --default-branch develop
```

**Via GitHub Web:**
1. Settings → Branches
2. Alterar default branch para `develop`

### 2.3: Configurar Branch Protection Rules

**Para branch `main`:**
```bash
# Via GitHub UI: Settings → Branches → Add rule
# Branch name pattern: main
```

Configurações recomendadas:
- ✅ Require a pull request before merging
- ✅ Require approvals (mínimo 1)
- ✅ Require status checks to pass before merging
- ✅ Require conversation resolution before merging
- ✅ Do not allow bypassing the above settings

**Para branch `develop`:**
- ✅ Require a pull request before merging
- ✅ Require status checks to pass before merging

---

## 🔐 Passo 3: Configurar Unity License para CI/CD

### 3.1: Gerar Arquivo de Ativação

```bash
# Disparar workflow de activation
gh workflow run activation.yml

# Aguardar conclusão (10-30 segundos)
gh run watch

# Listar runs recentes
gh run list --workflow=activation.yml --limit 1

# Baixar artifact (substitua RUN_ID pelo ID do run acima)
gh run download <RUN_ID>
```

### 3.2: Obter Unity License File

1. Extraia o arquivo `.alf` do artifact baixado
2. Acesse https://license.unity3d.com/manual
3. Faça upload do arquivo `.alf`
4. Faça login com sua conta Unity
5. Escolha a licença apropriada (Personal/Plus/Pro)
6. Baixe o arquivo `.ulf` gerado

### 3.3: Adicionar License como Secret

**Via GitHub CLI:**
```bash
# Adicionar conteúdo do arquivo .ulf como secret
gh secret set UNITY_LICENSE < caminho/para/arquivo.ulf
```

**Via GitHub Web:**
1. Settings → Secrets and variables → Actions
2. Click **"New repository secret"**
3. Name: `UNITY_LICENSE`
4. Value: Cole o conteúdo completo do arquivo `.ulf`
5. Click **"Add secret"**

### 3.4: Verificar Configuração

```bash
# Testar workflows manualmente
gh workflow run main.yml

# Monitorar execução
gh run watch
```

---

## 🎮 Passo 4: Configurar Projeto Unity

### 4.1: Abrir Projeto no Unity

1. Abrir **Unity Hub**
2. Click **"Add"** → **"Add project from disk"**
3. Selecionar pasta do repositório clonado
4. Abrir o projeto (pode demorar na primeira vez)

### 4.2: Configurar Nome e Identificação do Projeto

1. **Edit** → **Project Settings** → **Player**
2. Atualizar:
   - **Company Name**: Bugaboo Studio
   - **Product Name**: Nome do seu jogo
   - **Version**: `0.1.0` (inicial)

**Android:**
- Package Name: `com.bugaboostudio.seujogo`
- Version Code: `1`

**iOS:**
- Bundle Identifier: `com.bugaboostudio.seujogo`
- Build Number: `1`

### 4.3: Criar Estrutura de Diretórios

No Unity Editor:
1. **Assets** → **Create** → **Directory Structure**
2. Aguardar criação automática da estrutura padrão

Ou criar manualmente no Assets/:
```
Assets/
├── Animations/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── Voice/
├── Materials/
├── Models/
├── Prefabs/
├── Scenes/
├── Scripts/
├── Textures/
└── UI/
```

### 4.4: Remover Conteúdo de Template (Opcional)

Se não usar Ready Player Me:
```bash
# Remover do manifest.json
# Editar Packages/manifest.json e remover:
# - "com.readyplayerme.avatarloader"
# - "com.readyplayerme.core"
# - "com.readyplayerme.webview"

# Remover assets
rm -rf "Assets/Ready Player Me"
rm -rf "Assets/Samples/Ready Player Me Avatar Loader"
rm -rf "Assets/Samples/Ready Player Me WebView"
```

### 4.5: Configurar Quality Settings

1. **Edit** → **Project Settings** → **Quality**
2. Configurar níveis de qualidade para cada plataforma:
   - **Mobile** (iOS/Android): Low ou Medium
   - **Desktop** (Windows/Mac/Linux): High
   - **WebGL**: Medium

---

## 📝 Passo 5: Personalizar Documentação

### 5.1: Atualizar ReadMe.md

```bash
# Editar ReadMe.md
```

Substituir:
- Nome do projeto
- Descrição
- Links específicos
- Badges (opcional)

### 5.2: Atualizar CLAUDE.md

Adicionar seções específicas do projeto:
- Arquitetura específica
- SDKs utilizados
- Convenções de código do projeto

### 5.3: Atualizar CHANGELOG.md

```markdown
## [Unreleased]

## [0.1.0] - YYYY-MM-DD
### Added
- Initial project setup from template
- Basic project structure
```

---

## 🔧 Passo 6: Configurar Plataformas Específicas (Opcional)

### Android

Se vai fazer build para Android, configure:

```bash
# Gerar keystore
keytool -genkey -v -keystore android.keystore \
  -alias bugaboo-seujogo \
  -keyalg RSA -keysize 2048 -validity 10000

# Converter para base64
base64 android.keystore | tr -d '\n' > keystore-base64.txt

# Adicionar secrets
gh secret set ANDROID_KEYSTORE_BASE64 < keystore-base64.txt
gh secret set ANDROID_KEYSTORE_PASS --body "sua-senha"
gh secret set ANDROID_KEYALIAS_NAME --body "bugaboo-seujogo"
gh secret set ANDROID_KEYALIAS_PASS --body "sua-senha-alias"
```

**IMPORTANTE**: Guarde o keystore e senhas em local seguro!

### iOS

Para builds iOS, você precisará:
- Apple Developer Account
- Provisioning Profiles
- Certificates

Consulte documentação GameCI: https://game.ci/docs/github/deployment/ios

### WebGL

Para deploy em GitHub Pages:

```bash
# Habilitar GitHub Pages
gh repo edit --enable-pages --pages-branch gh-pages

# Ou via Web UI:
# Settings → Pages → Source → gh-pages branch
```

---

## 🧪 Passo 7: Testar CI/CD

### 7.1: Fazer Primeiro Commit

```bash
git add .
git commit -m "chore: initial project setup from template"
git push origin develop
```

### 7.2: Testar Workflows

```bash
# Disparar build manual
gh workflow run main.yml

# Monitorar
gh run watch

# Ver logs se falhar
gh run view --log
```

### 7.3: Criar Primeira Feature

```bash
# Criar feature branch
git checkout -b feature/player-controller

# Desenvolver...
# (adicionar cena, scripts, etc.)

# Commit seguindo Conventional Commits
git add .
git commit -m "feat: add basic player controller"

# Push
git push -u origin feature/player-controller

# Criar PR
gh pr create \
  --base develop \
  --title "feat: Add basic player controller" \
  --body "Adiciona controlador básico de player com WASD movement"
```

---

## 📦 Passo 8: Primeiro Release

Quando estiver pronto para primeiro release:

```bash
# Criar release branch
git checkout develop
git pull origin develop
git checkout -b release/v0.1.0

# Atualizar versão no Unity (Project Settings → Player → Version)
# Atualizar CHANGELOG.md

git add .
git commit -m "chore: bump version to 0.1.0"
git push -u origin release/v0.1.0

# Criar PR para main
gh pr create \
  --base main \
  --title "chore: Release v0.1.0" \
  --body "First release of [Nome do Jogo]"

# Após merge, criar tag
git checkout main
git pull origin main
git tag -a v0.1.0 -m "Release version 0.1.0"
git push origin v0.1.0

# Merge de volta para develop
git checkout develop
git merge main
git push origin develop
```

---

## ✅ Checklist Final

Antes de considerar setup completo, verifique:

### Git & GitHub
- [ ] Repositório criado a partir do template
- [ ] Branch `develop` criada e configurada como padrão
- [ ] Protected branches configuradas (main e develop)
- [ ] `.gitignore` verificado e funcional

### CI/CD
- [ ] `UNITY_LICENSE` secret adicionado
- [ ] Workflow `activation.yml` executado com sucesso
- [ ] Workflow `main.yml` executado com sucesso
- [ ] (Opcional) Secrets de plataforma configurados (Android, iOS)
- [ ] (Opcional) GitHub Pages habilitado para WebGL

### Unity
- [ ] Projeto abre sem erros no Unity Editor
- [ ] Nome e identificação do projeto configurados
- [ ] Estrutura de diretórios criada
- [ ] Quality settings configurados
- [ ] Conteúdo de template removido (se não usado)
- [ ] Primeira cena criada e salva

### Documentação
- [ ] ReadMe.md personalizado
- [ ] CLAUDE.md atualizado com especificidades do projeto
- [ ] CHANGELOG.md iniciado
- [ ] CODEOWNERS atualizado com teams corretos

### GitFlow
- [ ] Primeiro commit feito em develop
- [ ] Primeira feature branch criada e mergeada
- [ ] Equipe familiarizada com GitFlow workflow

---

## 🆘 Troubleshooting

### Erro: "Unity License is invalid"

**Solução:**
1. Verificar que secret `UNITY_LICENSE` está configurado
2. Verificar que conteúdo do secret é válido (arquivo .ulf completo)
3. Gerar nova license via workflow `activation.yml`

### Erro: "Library corrupted" no CI

**Solução:**
1. Limpar cache do GitHub Actions:
   ```bash
   # Via GitHub CLI (experimental)
   gh cache list
   gh cache delete <cache-id>
   ```
2. Ou via Web UI: Actions → Caches → Delete all caches

### Erro: Build falha em plataforma específica

**Solução:**
1. Verificar que módulos de build estão instalados no Unity
2. Verificar configurações específicas da plataforma
3. Consultar logs completos: `gh run view --log`

### Git Push 403 Error

**Solução:**
1. Verificar permissões no repositório
2. Verificar autenticação GitHub CLI: `gh auth status`
3. Re-autenticar se necessário: `gh auth login`

---

## 📞 Suporte

### Documentação
- **CLAUDE.md**: Guia completo do template
- **GameCI Docs**: https://game.ci/docs
- **Unity Docs**: https://docs.unity3d.com

### Contato
- **Issues**: https://github.com/bugaboostudio/ci_workflows_bugaboo/issues
- **Discussions**: https://github.com/bugaboostudio/ci_workflows_bugaboo/discussions
- **Email**: Contate time DevOps/Tech Lead da Bugaboo Studio

---

## 🎉 Próximos Passos

Após concluir o setup:

1. **Familiarize a equipe** com GitFlow workflow
2. **Configure code review** guidelines
3. **Adicione testes** unitários e de integração
4. **Configure notificações** de CI/CD (Slack, Discord, etc.)
5. **Estabeleça cadência** de releases
6. **Documente** decisões técnicas específicas do projeto

---

**Template Version**: 1.0.0
**Last Updated**: Janeiro 2026
**Maintainer**: Bugaboo Studio DevOps Team

---

✨ **Dica**: Mantenha este documento atualizado com descobertas e melhorias específicas do seu projeto!
