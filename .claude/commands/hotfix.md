---
description: Criar hotfix para correção urgente em produção
---

Você deve guiar o desenvolvedor através do processo de hotfix.

## Processo

1. **Verificar urgência**:
   - Confirmar que é realmente crítico
   - Hotfix é para produção (main branch)
   - Se não urgente, usar bugfix branch a partir de develop

2. **Criar hotfix branch**:
   ```bash
   git checkout main
   git pull origin main
   git checkout -b hotfix/v1.2.1  # ou hotfix/descricao-do-bug
   ```

3. **Fazer correção**:
   - Implementar fix mínimo necessário
   - Adicionar teste que reproduz o bug
   - Verificar que teste passa
   - Commit:
     ```bash
     git commit -am "fix: corrige crash ao carregar avatar"
     ```

4. **Testar**:
   - Build local
   - Testar fix
   - Rodar todos os testes

5. **Incrementar versão PATCH**:
   - v1.2.0 → v1.2.1
   - Atualizar ProjectSettings se necessário

6. **Finalizar hotfix**:
   ```bash
   # Merge para main
   git checkout main
   git merge hotfix/v1.2.1
   git tag -a v1.2.1 -m "Hotfix version 1.2.1"
   git push origin main --tags

   # Merge para develop também (importante!)
   git checkout develop
   git merge hotfix/v1.2.1
   git push origin develop

   # Excluir branch
   git branch -d hotfix/v1.2.1
   git push origin --delete hotfix/v1.2.1
   ```

7. **Deploy urgente**:
   - Trigger build de produção
   - Monitor deployment
   - Verificar que fix está funcionando

## Lembre-se

- Hotfix deve ser MÍNIMO e FOCADO
- Sempre merge para main E develop
- Testar bem antes de deploy
- Comunicar time sobre hotfix

Use o skill cicd-gitflow para mais detalhes sobre workflow de hotfix.
