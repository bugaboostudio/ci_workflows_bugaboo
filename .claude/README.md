# Claude Skills & Commands - CI Workflows Bugaboo

Este diretório contém skills e comandos personalizados para assistir no desenvolvimento deste projeto Unity.

## 🎯 Skills Disponíveis

### 1. cicd-gitflow

**Descrição**: Assistente especializado em CI/CD e GitFlow para projetos Unity.

**Quando usar**:
- Criar branches (feature, release, hotfix)
- Escrever conventional commits
- Otimizar workflows GitHub Actions
- Entender versionamento semântico
- Troubleshooting de builds CI/CD

**Exemplos**:
- "Como criar uma feature branch?"
- "Como fazer uma release v1.2.0?"
- "Meu workflow está lento, como otimizar?"

---

### 2. unity-dev

**Descrição**: Assistente de desenvolvimento Unity 3D e C# scripting.

**Quando usar**:
- MonoBehaviour lifecycle
- Performance optimization
- Unity patterns e best practices
- Ready Player Me integration
- Cross-platform development

**Exemplos**:
- "Como usar MonoBehaviour corretamente?"
- "Como otimizar Update()?"
- "Como integrar Ready Player Me?"

---

### 3. testing

**Descrição**: Assistente de testes e TDD para Unity.

**Quando usar**:
- Escrever unit tests
- Implementar TDD workflow
- Criar mocks e stubs
- Testar MonoBehaviours
- Aumentar code coverage

**Exemplos**:
- "Como testar uma MonoBehaviour?"
- "Como fazer TDD em Unity?"
- "Como criar mocks?"

---

### 4. code-review

**Descrição**: Assistente de code review e quality assurance.

**Quando usar**:
- Fazer review de PR
- Identificar code smells
- Security review
- Performance analysis
- Sugestões de refatoração

**Exemplos**:
- "Revise minhas mudanças"
- "Este código tem problemas de segurança?"
- "Como melhorar este código?"

---

### 5. performance

**Descrição**: Assistente de otimização de performance Unity.

**Quando usar**:
- Profiling e análise
- Otimizar CPU/GPU
- Reduzir draw calls
- Memory optimization
- Mobile/WebGL performance

**Exemplos**:
- "Como otimizar este script?"
- "Por que tenho muitos draw calls?"
- "Como reduzir uso de memória?"

---

## ⚡ Comandos Slash Disponíveis

### /review

Fazer code review completo das mudanças atuais.

```bash
/review
```

**O que faz**:
- Analisa mudanças no git
- Identifica bugs e code smells
- Verifica performance e security
- Fornece feedback detalhado

---

### /test

Rodar testes Unity e analisar resultados.

```bash
/test
```

**O que faz**:
- Lista testes existentes
- Executa testes (ou orienta como executar)
- Analisa resultados
- Sugere novos testes

---

### /feature

Criar nova feature branch seguindo GitFlow.

```bash
/feature
```

**O que faz**:
- Pergunta nome da feature
- Cria branch feature/nome
- Orienta sobre conventional commits
- Guia próximos passos

---

### /release

Iniciar processo de release seguindo GitFlow.

```bash
/release
```

**O que faz**:
- Confirma versão (SemVer)
- Cria release branch
- Guia preparação de release
- Orienta merge e tagging

---

### /hotfix

Criar hotfix para correção urgente em produção.

```bash
/hotfix
```

**O que faz**:
- Cria hotfix branch a partir de main
- Guia implementação da correção
- Orienta merge para main e develop
- Ajuda com deploy urgente

---

### /optimize

Analisar código e fornecer sugestões de otimização.

```bash
/optimize
```

**O que faz**:
- Identifica problemas de performance
- Analisa scripts e assets
- Fornece relatório detalhado
- Sugere quick wins

---

### /build

Preparar e executar build para plataforma específica.

```bash
/build
```

**O que faz**:
- Confirma plataforma alvo
- Verifica pre-build checklist
- Orienta build local ou CI/CD
- Verifica build final

---

### /docs

Atualizar documentação do projeto.

```bash
/docs
```

**O que faz**:
- Identifica o que documentar
- Atualiza README, CLAUDE.md
- Adiciona code comments
- Mantém CHANGELOG

---

## 📁 Estrutura de Arquivos

```
.claude/
├── README.md                      # Este arquivo
├── skills/                        # Skills de AI assistant
│   ├── cicd-gitflow.md           # CI/CD e GitFlow
│   ├── cicd-gitflow.json
│   ├── unity-dev.md              # Desenvolvimento Unity
│   ├── unity-dev.json
│   ├── testing.md                # Testes e TDD
│   ├── testing.json
│   ├── code-review.md            # Code review
│   ├── code-review.json
│   ├── performance.md            # Performance optimization
│   └── performance.json
└── commands/                      # Comandos slash
    ├── review.md                 # /review
    ├── test.md                   # /test
    ├── feature.md                # /feature
    ├── release.md                # /release
    ├── hotfix.md                 # /hotfix
    ├── optimize.md               # /optimize
    ├── build.md                  # /build
    └── docs.md                   # /docs
```

## 🚀 Como Usar

### Skills

Skills são ativados automaticamente quando você faz perguntas relacionadas:

```
"Como otimizar este código?"           → skill: performance
"Como testar este MonoBehaviour?"      → skill: testing
"Crie uma feature branch"              → skill: cicd-gitflow
```

Ou invoque explicitamente:
```
@skill unity-dev
```

### Comandos Slash

Use comandos slash digitando `/` seguido do comando:

```
/review      # Code review
/test        # Rodar testes
/feature     # Nova feature
/optimize    # Otimização
```

## 💡 Workflows Comuns

### Iniciar Nova Feature

```bash
1. /feature
   → Cria branch feature/nome
2. Desenvolver...
3. /test
   → Verifica testes
4. /review
   → Code review
5. Criar PR para develop
```

### Preparar Release

```bash
1. /release
   → Cria release branch
2. /test
   → Rodar todos os testes
3. /build
   → Build para todas as plataformas
4. /docs
   → Atualizar documentação
5. Merge e tag release
```

### Correção Urgente

```bash
1. /hotfix
   → Cria hotfix branch
2. Implementar correção
3. /test
   → Verificar fix
4. /build
   → Build urgente
5. Deploy imediato
```

### Otimização de Performance

```bash
1. /optimize
   → Identificar problemas
2. Implementar otimizações
3. /test
   → Verificar que não quebrou
4. Profile novamente
5. /review antes de merge
```

## 🛠️ Como Adicionar Novos Skills

1. **Criar skill markdown**:
   ```bash
   .claude/skills/meu-skill.md
   ```

2. **Criar metadata JSON**:
   ```bash
   .claude/skills/meu-skill.json
   ```

3. **Atualizar README**:
   Adicionar documentação do skill neste arquivo

4. **Commit**:
   ```bash
   git add .claude/
   git commit -m "feat(skills): adiciona skill meu-skill"
   ```

### Template de Skill

**meu-skill.md**:
```markdown
# Meu Skill Name

Você é um assistente especializado em [área].

## Sua Função
[Descrever o que o skill faz]

## Quando Usar
[Quando invocar este skill]

## Exemplos
[Exemplos práticos]
```

**meu-skill.json**:
```json
{
  "name": "meu-skill",
  "description": "Breve descrição",
  "version": "1.0.0",
  "author": "Bugaboo Studio",
  "tags": ["tag1", "tag2"],
  "type": "user"
}
```

## 🛠️ Como Adicionar Novos Comandos

1. **Criar comando markdown**:
   ```bash
   .claude/commands/meu-comando.md
   ```

2. **Formato do comando**:
   ```markdown
   ---
   description: Breve descrição do que faz
   ---

   Instruções detalhadas...
   ```

3. **Atualizar README**:
   Documentar o comando neste arquivo

4. **Testar**:
   ```bash
   /meu-comando
   ```

## 📚 Referências

### Documentação Oficial

- **Claude Code**: [github.com/anthropics/claude-code](https://github.com/anthropics/claude-code)
- **Unity Manual**: [docs.unity3d.com](https://docs.unity3d.com)
- **GameCI**: [game.ci/docs](https://game.ci/docs)
- **GitFlow**: [nvie.com/posts/a-successful-git-branching-model](https://nvie.com/posts/a-successful-git-branching-model/)
- **Conventional Commits**: [conventionalcommits.org](https://www.conventionalcommits.org/)

### Documentação do Projeto

- **CLAUDE.md**: Guia completo para AI e desenvolvedores
- **README.md**: Overview do projeto Unity
- **.github/workflows/**: Workflows de CI/CD

## 🤝 Contribuindo

Para melhorar skills ou comandos:

1. **Criar branch**:
   ```bash
   git checkout -b feature/improve-skill-name
   ```

2. **Fazer mudanças**:
   - Editar skills em `.claude/skills/`
   - Editar comandos em `.claude/commands/`
   - Atualizar este README

3. **Commit**:
   ```bash
   git commit -m "feat(skills): melhora skill de testing"
   ```

4. **PR para develop**

## 📋 Changelog

### v1.1.0 - 2026-01-05

**Adicionado**:
- Skills: unity-dev, testing, code-review, performance
- Comandos: /review, /test, /feature, /release, /hotfix, /optimize, /build, /docs
- README completo com documentação de todos os skills e comandos

### v1.0.0 - 2026-01-05

**Adicionado**:
- Skill inicial: cicd-gitflow
- Estrutura base de .claude/

---

**Última atualização**: 2026-01-05
**Versão**: 1.1.0
**Mantido por**: Bugaboo Studio
