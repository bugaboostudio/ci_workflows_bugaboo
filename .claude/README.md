# Claude Skills - CI Workflows Bugaboo

Este diretório contém skills personalizados para assistir no desenvolvimento deste projeto Unity.

## 🎯 Skill Disponível

### cicd-gitflow

**Descrição**: Assistente especializado em CI/CD e GitFlow para projetos Unity da Bugaboo Studio.

**Quando usar**:
- Precisa de orientação sobre qual tipo de branch criar
- Dúvidas sobre como escrever conventional commits
- Quer otimizar workflows de CI/CD
- Precisa entender estratégias de versionamento
- Tem dúvidas sobre GitFlow workflow
- Precisa de templates de PR ou commit messages
- Troubleshooting de builds CI/CD

**Como usar**:

O skill é ativado automaticamente quando você faz perguntas relacionadas:

```
"Como criar uma feature branch?"
"Como escrevo um commit para bugfix?"
"Quero fazer uma release v1.2.0, qual o processo?"
"Meu workflow está muito lento, como otimizar?"
"Meu build está falhando no GitHub Actions, o que verificar?"
```

## 📁 Estrutura de Arquivos

```
.claude/
├── README.md                      # Este arquivo
└── skills/
    ├── cicd-gitflow.md           # Conteúdo do skill
    └── cicd-gitflow.json         # Metadados do skill
```

## 📚 O que o Skill Cobre

### GitFlow Workflow
- Estrutura de branches (main, develop, feature, release, hotfix)
- Nomenclatura de branches
- Workflows completos de feature, release e hotfix
- Estratégias de merge

### Conventional Commits
- Tipos de commit (feat, fix, docs, refactor, test, chore, ci, perf)
- Formatação de mensagens
- Breaking changes
- Exemplos práticos

### CI/CD com GameCI
- Otimização de workflows GitHub Actions
- Estratégias de cache para Unity
- Builds paralelos
- Secrets necessários
- Troubleshooting comum
- Performance optimization

### Versionamento Semântico
- MAJOR.MINOR.PATCH
- Quando incrementar cada parte
- Tagging de releases

### Templates e Best Practices
- Template de Pull Request
- Checklist de PR
- Comandos git úteis
- Comandos GitHub CLI
- Protected branches
- Code review workflow

## 🛠️ Como Adicionar Novos Skills

1. **Criar skill markdown**:
   ```bash
   .claude/skills/nome-do-skill.md
   ```

2. **Criar metadata JSON**:
   ```bash
   .claude/skills/nome-do-skill.json
   ```

3. **Atualizar README**:
   Adicionar documentação do skill neste arquivo

4. **Commit**:
   ```bash
   git add .claude/
   git commit -m "feat(skills): adiciona skill nome-do-skill"
   ```

### Template de Skill

**nome-do-skill.md**:
```markdown
# Nome do Skill

Você é um assistente especializado em [área].

## Sua Função
[Descrever função]

## Regras
[Listar regras e guidelines]

## Exemplos
[Fornecer exemplos práticos]
```

**nome-do-skill.json**:
```json
{
  "name": "nome-do-skill",
  "description": "Breve descrição",
  "version": "1.0.0",
  "author": "Bugaboo Studio",
  "tags": ["tag1", "tag2"],
  "type": "user"
}
```

## 🤝 Contribuindo

Para melhorar ou adicionar skills:

1. Criar branch: `git checkout -b feature/add-skill-nome`
2. Adicionar/modificar skills em `.claude/skills/`
3. Atualizar este README.md
4. Commitar: `git commit -m "feat(skills): adiciona skill nome"`
5. Criar PR para `develop`

## 📚 Referências

- [Claude Code Skills Documentation](https://github.com/anthropics/claude-code)
- [CLAUDE.md](../CLAUDE.md) - Documentação completa do projeto
- [GameCI Documentation](https://game.ci/docs)
- [GitFlow Original](https://nvie.com/posts/a-successful-git-branching-model/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)

---

**Última atualização**: 2026-01-05
**Versão**: 1.0.0
**Mantido por**: Bugaboo Studio
