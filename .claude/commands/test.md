---
description: Rodar testes Unity e analisar resultados
---

Você deve rodar os testes do projeto e analisar os resultados.

## Processo

1. **Verificar testes existentes**:
   - Procurar por testes em `Assets/Tests/`
   - Listar EditMode e PlayMode tests

2. **Executar testes** (escolher método):

   **Opção A - Via Unity CLI**:
   ```bash
   # EditMode tests
   unity-editor -runTests -batchmode -projectPath . -testResults TestResults-EditMode.xml -testPlatform EditMode

   # PlayMode tests
   unity-editor -runTests -batchmode -projectPath . -testResults TestResults-PlayMode.xml -testPlatform PlayMode
   ```

   **Opção B - Sugerir usar Unity Test Runner**:
   - Window > General > Test Runner
   - Rodar testes no Editor

   **Opção C - Via GitHub Actions**:
   - Trigger workflow de testes

3. **Analisar resultados**:
   - Quantos testes passaram/falharam
   - Quais testes falharam e por quê
   - Code coverage (se disponível)
   - Performance de testes

4. **Fornecer recomendações**:
   - Testes faltando para código novo
   - Melhorias em testes existentes
   - Testes que podem ser otimizados

Use o skill testing para orientação sobre como escrever melhores testes.
