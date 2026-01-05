---
description: Analisar código e fornecer sugestões de otimização
---

Você deve analisar o código do projeto e fornecer sugestões de otimização de performance.

## Processo

1. **Identificar área de análise**:
   - Perguntar ao usuário: "Qual área deseja otimizar?"
     - Geral (todo o projeto)
     - Scripts específicos
     - Rendering/GPU
     - Memory
     - Build size

2. **Análise de Scripts**:
   - Procurar por padrões problemáticos:
     - GetComponent em Update/FixedUpdate
     - Find/FindObjectOfType em loops
     - String concatenation em hot paths
     - Alocações desnecessárias
     - Falta de object pooling

3. **Análise de Assets**:
   - Texturas não comprimidas
   - Audio não comprimido
   - Meshes com muitos vértices
   - Materiais duplicados

4. **Análise de Cena**:
   - Muitos draw calls
   - Objetos sem LOD
   - Falta de occlusion culling
   - Iluminação não baked

5. **Fornecer relatório**:
   ```markdown
   ## Análise de Performance

   ### 🚨 Issues Críticos
   - [Lista de problemas que impactam muito]

   ### ⚠️ Issues Moderados
   - [Lista de problemas moderados]

   ### 💡 Sugestões
   - [Lista de melhorias opcionais]

   ### 📊 Quick Wins
   - [Otimizações rápidas com alto impacto]

   ### 🎯 Próximos Passos
   1. [Prioridade 1]
   2. [Prioridade 2]
   3. [Prioridade 3]
   ```

6. **Fornecer exemplos de código**:
   - Mostrar código problemático
   - Mostrar código otimizado
   - Explicar o ganho esperado

Use o skill performance para orientação detalhada sobre otimização.
