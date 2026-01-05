---
description: Fazer code review completo das mudanças atuais
---

Você deve fazer um code review detalhado das mudanças no repositório.

## Processo

1. **Analisar mudanças**:
   - Use `git diff` para ver alterações staged/unstaged
   - Use `git diff develop...HEAD` para ver todas as mudanças do branch

2. **Revisar cada arquivo**:
   - Verificar funcionalidade
   - Identificar bugs potenciais
   - Verificar performance
   - Verificar security issues
   - Verificar code quality

3. **Checklist**:
   - [ ] Código compila sem erros
   - [ ] Sem warnings
   - [ ] Testes passam
   - [ ] Sem code smells
   - [ ] Performance adequada
   - [ ] Security OK
   - [ ] Documentação atualizada

4. **Fornecer feedback**:
   - Listar issues encontrados (Blocker, Serious, Suggestion)
   - Sugerir melhorias
   - Dar rating geral (Approve/Request Changes/Comment)

Use o skill code-review para orientação detalhada.
