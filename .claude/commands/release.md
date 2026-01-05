---
description: Iniciar processo de release seguindo GitFlow
---

Você deve guiar o desenvolvedor através do processo de release.

## Processo

1. **Verificar estado**:
   ```bash
   git status
   git branch
   ```

2. **Confirmar com usuário**:
   - Qual versão será released? (ex: v1.2.0)
   - Seguir Semantic Versioning:
     - MAJOR: Breaking changes
     - MINOR: Nova funcionalidade
     - PATCH: Bugfixes

3. **Criar release branch**:
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b release/v1.2.0
   ```

4. **Tarefas de preparação**:
   - [ ] Atualizar versão no Unity (ProjectSettings/ProjectSettings.asset)
   - [ ] Atualizar CHANGELOG.md (se existir)
   - [ ] Fazer bugfixes finais (NENHUMA nova feature!)
   - [ ] Rodar testes
   - [ ] Build de todas as plataformas

5. **Commit de versão**:
   ```bash
   git commit -am "chore: bump version to 1.2.0"
   ```

6. **Finalizar release**:
   ```bash
   # Merge para main
   git checkout main
   git pull origin main
   git merge release/v1.2.0
   git tag -a v1.2.0 -m "Release version 1.2.0"
   git push origin main --tags

   # Merge de volta para develop
   git checkout develop
   git merge release/v1.2.0
   git push origin develop

   # Excluir branch
   git branch -d release/v1.2.0
   git push origin --delete release/v1.2.0
   ```

7. **Trigger deployment**:
   - GitHub Actions pode automaticamente buildar a tag
   - Deploy para stores se configurado

Use o skill cicd-gitflow para orientação detalhada sobre versionamento e releases.
