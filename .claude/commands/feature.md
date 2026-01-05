---
description: Criar nova feature branch seguindo GitFlow
---

Você deve ajudar o desenvolvedor a criar uma nova feature branch corretamente.

## Processo

1. **Perguntar ao usuário**:
   - Qual o nome descritivo da feature?
   - Há issue/ticket relacionado? (ex: JIRA-123)

2. **Verificar estado atual**:
   ```bash
   git status
   git branch
   ```

3. **Atualizar develop**:
   ```bash
   git checkout develop
   git pull origin develop
   ```

4. **Criar feature branch**:
   - Nomenclatura: `feature/nome-descritivo` ou `feature/ISSUE-123-descricao`
   - Exemplo: `feature/inventory-system`, `feature/GAME-456-multiplayer`

   ```bash
   git checkout -b feature/[nome]
   ```

5. **Push inicial** (se desejado):
   ```bash
   git push -u origin feature/[nome]
   ```

6. **Orientar próximos passos**:
   - Desenvolver a feature
   - Commitar regularmente com conventional commits
   - Quando pronto, criar PR para develop

## Exemplo de Conventional Commit

```
feat(inventory): adiciona sistema de inventário

- Adiciona classe Inventory
- Implementa add/remove items
- Cria UI de inventário
```

Use o skill cicd-gitflow para orientação detalhada sobre GitFlow e conventional commits.
