# Unity CI/CD Template - Bugaboo Studio 🎮

[![GameCI](https://img.shields.io/badge/GameCI-Enabled-blue)](https://game.ci/)
[![Unity](https://img.shields.io/badge/Unity-2022.3+-black)](https://unity.com/)
[![License](https://img.shields.io/badge/License-Internal-green)](LICENSE)
[![Template Version](https://img.shields.io/badge/Template-v1.0.0-orange)](CHANGELOG.md)
[![GitFlow](https://img.shields.io/badge/GitFlow-Enabled-brightgreen)](CLAUDE.md#gitflow-workflow-template-best-practices)
[![CI Status](https://img.shields.io/badge/CI-passing-success)](../../actions)
[![Security](https://img.shields.io/badge/Security-Policy-red)](SECURITY.md)
[![Setup Guide](https://img.shields.io/badge/Setup-Guide-informational)](TEMPLATE_SETUP.md)

**Template oficial de projetos Unity** com CI/CD usando GitHub Actions e GameCI.

Este repositório serve como **ponto de partida** para novos projetos Unity na Bugaboo Studio, com workflows de CI/CD já configurados, estrutura de pastas organizada, e best practices implementadas.

---

## 📋 Índice

- [Sobre o Template](#sobre-o-template)
- [Recursos Incluídos](#recursos-incluídos)
- [Início Rápido](#início-rápido)
- [Documentação Completa](#documentação-completa)
- [Contribuindo](#contribuindo)
- [Suporte](#suporte)

---

## 🎯 Sobre o Template

Este template foi criado para:

✅ **Acelerar início de novos projetos** - Não perca tempo configurando CI/CD
✅ **Garantir qualidade** - Tests automáticos e code review
✅ **Padronizar workflows** - Mesmas práticas em todos os projetos
✅ **Facilitar manutenção** - Estrutura consistente e documentada
✅ **Suportar múltiplas plataformas** - Windows, macOS, Linux, iOS, Android, WebGL

---

## 🚀 Recursos Incluídos

### CI/CD Workflows
- ✅ **Testes Automáticos** - EditMode e PlayMode
- ✅ **Build Multi-Plataforma** - 6 plataformas simultâneas
- ✅ **Deploy Automático** - WebGL para GitHub Pages
- ✅ **Code Inspection** - ReSharper integration
- ✅ **Unity License Management** - Workflow de ativação incluído

### Estrutura de Projeto
- 📁 Estrutura de pastas padronizada e organizada
- 📝 Editor script para criar estrutura automaticamente
- ⚙️ Configurações de quality (Low/Medium/High)
- 🎨 Ready Player Me SDK integrado (opcional)

### Documentação
- 📚 **CLAUDE.md** - Guia completo para AI assistants e desenvolvedores
- 📖 **CONTRIBUTING.md** - Guidelines de contribuição
- 📋 **CHANGELOG.md** - Histórico de versões do template
- 📝 Template de Pull Request
- 🔧 Configuração Dependabot

### GitFlow & Best Practices
- 🌿 GitFlow workflow implementado
- 📝 Conventional Commits
- 🏷️ Semantic Versioning
- 👥 Code Owners configurável
- 🔒 Branch protection guidelines

---

## 🏁 Início Rápido

### Usando Este Template para Novo Projeto

#### 1. Criar Repositório a Partir do Template

**Opção A - GitHub UI:**
1. Clique em "Use this template" (botão verde no topo)
2. Escolha nome e visibilidade do novo repositório
3. Clique em "Create repository"

**Opção B - GitHub CLI:**
```bash
gh repo create meu-novo-jogo \
  --template bugaboostudio/ci_workflows_bugaboo \
  --private \
  --clone
cd meu-novo-jogo
```

#### 2. Configuração Inicial (5 minutos)

```bash
# Criar branch develop
git checkout -b develop
git push -u origin develop

# Configurar develop como branch padrão no GitHub
gh repo edit --default-branch develop
```

#### 3. Configurar Unity License (10 minutos)

```bash
# 1. Disparar workflow de activation
gh workflow run activation.yml

# 2. Aguardar workflow completar (alguns segundos)

# 3. Baixar artifact
gh run download

# 4. Fazer upload do arquivo em: https://license.unity3d.com/manual

# 5. Adicionar license como secret
# GitHub UI: Settings > Secrets > Actions > New repository secret
# Nome: UNITY_LICENSE
# Valor: [Conteúdo completo do arquivo .ulf]
```

#### 4. Abrir na Unity e Personalizar

1. Abrir Unity Hub
2. Add project from disk → Selecionar pasta clonada
3. Abrir o projeto
4. **Unity Menu** → `Assets > Create > Directory Structure` (se necessário)
5. Personalizar conforme necessidade do projeto

#### 5. Primeiro Commit

```bash
git add .
git commit -m "chore: initial project setup from template"
git push origin develop
```

#### 6. Criar Primeira Feature

```bash
git checkout -b feature/player-movement
# ... desenvolver ...
git commit -am "feat: add basic player movement"
git push -u origin feature/player-movement
# Criar PR no GitHub UI
```

### ✅ Pronto! Seu Projeto Está Configurado

Agora você tem:
- ✅ Repositório com GitFlow
- ✅ CI/CD funcionando
- ✅ Estrutura de pastas organizada
- ✅ Documentação completa

---

## 📚 Documentação Completa

Para informações detalhadas, consulte:

### Para Desenvolvedores
- **[CLAUDE.md](CLAUDE.md)** - Guia completo do projeto
  - Estrutura do projeto
  - GitFlow workflow detalhado
  - CI/CD best practices
  - Como usar o template
  - Troubleshooting

### Para Contribuidores
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Como contribuir
  - Processo de desenvolvimento
  - Style guide
  - Pull request process
  - Conventional commits

### Histórico
- **[CHANGELOG.md](CHANGELOG.md)** - Versões e mudanças do template

---

## 🤝 Contribuindo

Quer melhorar este template? Contribuições são bem-vindas!

### Como Contribuir

1. Fork este repositório
2. Crie uma branch de feature (`git checkout -b feature/amazing-feature`)
3. Commit suas mudanças (`git commit -m 'feat: add amazing feature'`)
4. Push para a branch (`git push origin feature/amazing-feature`)
5. Abra um Pull Request

**Importante:** Leia [CONTRIBUTING.md](CONTRIBUTING.md) antes de contribuir.

### Tipos de Contribuições

- 🐛 Reportar bugs
- 💡 Sugerir melhorias
- 📝 Melhorar documentação
- 🔧 Adicionar/melhorar workflows
- ⚡ Otimizações de performance

---

## 📞 Suporte

### Documentação

- **GameCI**: https://game.ci/docs/github/getting-started
- **Unity Actions**: https://github.com/game-ci/unity-actions
- **CLAUDE.md**: Guia completo do projeto

### Contato

- **Issues**: [Reportar problemas](https://github.com/bugaboostudio/ci_workflows_bugaboo/issues)
- **Discussions**: [Discussões do projeto](https://github.com/bugaboostudio/ci_workflows_bugaboo/discussions)
- **Email**: Contate Tiago (Bugaboo Studio) para questões de CI/CD

### Links Úteis

- [Game.ci - Primeiros Passos](https://game.ci/docs/github/getting-started)
- [Github "Unity Actions" Project](https://github.com/game-ci/unity-actions)
- [Unity License Activation](https://game.ci/docs/github/activation)

---

## 📋 Checklist de Configuração Inicial

Use este checklist ao configurar um novo projeto a partir deste template:

- [ ] Criar repositório a partir do template
- [ ] Clonar repositório localmente
- [ ] Criar branch `develop`
- [ ] Configurar Unity license no GitHub Secrets
- [ ] Configurar protected branches (main e develop)
- [ ] Atualizar `ReadMe.md` com informações do projeto
- [ ] Executar `Assets > Create > Directory Structure` na Unity
- [ ] Remover packages não necessários (ex: Ready Player Me se não usar)
- [ ] Adicionar packages específicos do projeto
- [ ] Configurar quality settings para plataformas alvo
- [ ] Criar `.github/ISSUE_TEMPLATE/` se necessário
- [ ] Atualizar `.github/CODEOWNERS` com teams/usernames corretos
- [ ] Adicionar secrets de plataforma (Android keystore, iOS certificates)
- [ ] Testar workflows (rodar `main.yml` manualmente)
- [ ] Configurar GitHub Pages para WebGL (se aplicável)
- [ ] Atualizar `CLAUDE.md` com especificidades do projeto
- [ ] Criar primeiro release (`v0.1.0`)

**Checklist completo**: Veja [CLAUDE.md - Usando Como Template](CLAUDE.md#usando-este-repositório-como-template)

---

## 🏷️ Versões

- **Template Version**: 1.0.0 ([Changelog](CHANGELOG.md))
- **Unity Version**: 2022.3+ (LTS recomendado)
- **GameCI Actions**: Latest

---

## 📄 Licença

Este template é de uso interno da Bugaboo Studio.

---

## 🙏 Agradecimentos

- [GameCI](https://game.ci/) - CI/CD para Unity
- [Unity Technologies](https://unity.com/) - Game Engine
- Comunidade Unity - Suporte e recursos

---

## 🗂️ Estrutura de Arquivos

```
ci_workflows_bugaboo/
├── .github/
│   ├── workflows/              # GitHub Actions CI/CD
│   │   ├── main.yml            # Pipeline principal
│   │   ├── activation.yml      # Unity license activation
│   │   └── ...
│   ├── CODEOWNERS              # Code ownership
│   ├── dependabot.yml          # Dependency updates
│   └── PULL_REQUEST_TEMPLATE.md
├── Assets/
│   ├── Animations/             # Animation files
│   ├── Editor/                 # Editor scripts
│   ├── Models/                 # 3D models
│   ├── Scenes/                 # Unity scenes
│   ├── Scripts/                # C# scripts
│   └── ...
├── Packages/
│   └── manifest.json           # Unity packages
├── ProjectSettings/            # Unity settings
├── CHANGELOG.md                # Template changelog
├── CLAUDE.md                   # AI/Developer guide
├── CONTRIBUTING.md             # Contribution guidelines
└── ReadMe.md                   # Este arquivo
```

---

<div align="center">

**Feito com ❤️ pela [Bugaboo Studio](https://bugaboostudio.com)**

⭐ Se este template foi útil, considere dar uma estrela!

</div>

---

# Documentação Original GameCI

> **Nota**: A documentação abaixo é referência para configuração manual.
> Se você está usando este template, a maior parte já está configurada!

---

## Por favor, leia a documentação completa em:

https://game.ci/docs/github/getting-started

## Links

 - [Game.ci - Primeiros Passos](https://game.ci/docs/github/getting-started)
 - [Github "Unity Actions" Project](https://github.com/game-ci/unity-actions)


## Passos para configurar o fluxo de trabalho do Game CI para um novo repositório unity.

### Criando um projeto Unity.

 - Digite na pasta raiz do projeto: `git init`
 - Adicione seu git ao nosso github `git remote add origin ...`
 - Pegue arquivo .gitignore de [gitignore.io](https://www.toptal.com/developers/gitignore/api/unity)
 - git add and commit and push
 - Create your actions workflow: `.github/workflows/main.yml`
 - git add and commit and push


### Crie uma Licença
 - [DOCUMENTATION](https://game.ci/docs/github/activation)
 - Coloque para rodar o workflow activation: `.github/workflows/activation.yml` ver o documento acima.
 - Check that your workflow passed.
 - Download `Manual Activation File` artifact.
 - Extract zip.
 - Faça o upload do arquivo para: [license generate](https://license.unity3d.com/manual)
 - Download `Unity_*.ulf` file


### faça um update da variável 

 - Adicione uma variável secreta para seu repositório.
   - `UNITY_LICENSE` - The content of the file you just downloaded


### Customise your workflow to run tests.
 - [DOCUMENTATION](https://game.ci/docs/github/test-runner)
 - Edit main.yml.
 - Add edit/play/all test jobs to you workflow.
 - git add and commit and push

### Customise your workflow to build games.
 - [DOCUMENTATION](https://game.ci/docs/github/builder)
 - Edit main.yml.
 - Add jobs for your build targets to the workflow.
 - (optional) Make build jobs dependent on tests passing.
 - git add and commit and push
 - Download Standalone build or Android build to test.

### Customise your workflow to deploy games.
 - Edit main.yml.
 - Add a job for deploying to github pages.
 - Make deploy job dependent on the webgl build passing.
 - git add and commit and push
 - Go to github pages settings and select which branch has your pages webgl deployment (eg. `gh-pages`)
 - Go to `https://[your-github-name].github.io/[your-repo-name]` to test your web build.
