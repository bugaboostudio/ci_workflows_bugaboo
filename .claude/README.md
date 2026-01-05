# Claude Skills - CI Workflows Bugaboo

Este diretório contém skills personalizados para assistir no desenvolvimento deste projeto Unity.

## Skills Disponíveis

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

**Como invocar**:
```
@skill cicd-gitflow
```

Ou simplesmente pergunte sobre CI/CD ou GitFlow e o skill será ativado automaticamente quando relevante.

**Exemplos de uso**:

1. **Criar nova feature**:
   ```
   Como devo criar uma branch para adicionar sistema de inventário?
   ```

2. **Escrever commit**:
   ```
   Como escrevo um commit para correção de bug de UI?
   ```

3. **Preparar release**:
   ```
   Quero fazer uma release v1.2.0, qual o processo?
   ```

4. **Otimizar CI/CD**:
   ```
   Meu workflow está muito lento, como otimizar?
   ```

5. **Troubleshooting**:
   ```
   Meu build está falhando no GitHub Actions, o que verificar?
   ```

## Estrutura de Arquivos

```
.claude/
├── README.md                      # Este arquivo
└── skills/                        # Diretório de skills
    ├── cicd-gitflow.md           # Conteúdo do skill
    └── cicd-gitflow.json         # Metadados do skill
```

## Como Adicionar Novos Skills

1. Criar arquivo `.md` em `.claude/skills/` com o conteúdo do skill
2. Criar arquivo `.json` correspondente com metadados
3. Atualizar este README.md com documentação do novo skill

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

## Contribuindo

Para melhorar ou adicionar skills:

1. Criar branch: `git checkout -b feature/add-skill-nome`
2. Adicionar/modificar skills em `.claude/skills/`
3. Atualizar este README.md
4. Commitar: `git commit -m "feat(skills): adiciona skill nome"`
5. Criar PR para `develop`

## Referências

- [Claude Code Skills Documentation](https://github.com/anthropics/claude-code)
- [CLAUDE.md](../CLAUDE.md) - Documentação completa do projeto
- [GameCI Documentation](https://game.ci/docs)
- [GitFlow Original](https://nvie.com/posts/a-successful-git-branching-model/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Última atualização**: 2026-01-05
**Versão**: 1.0.0
