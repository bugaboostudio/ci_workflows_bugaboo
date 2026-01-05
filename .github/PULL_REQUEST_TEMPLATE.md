## 📝 Descrição

<!-- Descreva as mudanças de forma clara e concisa -->
<!-- O que foi alterado e por quê? -->

## 🎯 Tipo de Mudança

Marque as opções relevantes:

- [ ] 🚀 Nova feature (mudança não-breaking que adiciona funcionalidade)
- [ ] 🐛 Bugfix (mudança não-breaking que corrige um issue)
- [ ] 💥 Breaking change (fix ou feature que causa mudança em funcionalidade existente)
- [ ] 📚 Documentação (melhorias ou correções em documentação)
- [ ] 🎨 Refatoração (mudanças de código que não alteram funcionalidade)
- [ ] ⚡ Performance (mudanças que melhoram performance)
- [ ] ✅ Testes (adição ou correção de testes)
- [ ] 🔧 Chore (mudanças em build process, dependências, etc.)
- [ ] 🔒 Segurança (correção de vulnerabilidade)

## 🔗 Issues Relacionadas

<!-- Link para issues usando: Closes #123, Fixes #456, Relates to #789 -->

Closes #

## 🧪 Como Testar

<!-- Descreva os passos para testar as mudanças -->

1.
2.
3.

**Plataformas testadas:**
- [ ] Windows
- [ ] macOS
- [ ] Linux
- [ ] Android
- [ ] iOS
- [ ] WebGL

## 📸 Screenshots/Videos

<!-- Se aplicável, adicione screenshots ou videos das mudanças -->
<!-- Especialmente importante para mudanças de UI ou visuais -->

## ✅ Checklist

Antes de submeter o PR, verifique:

### Código
- [ ] Meu código segue o style guide do projeto
- [ ] Fiz self-review do meu código
- [ ] Comentei partes complexas do código, especialmente áreas não-óbvias
- [ ] Removi código comentado e debug logs desnecessários
- [ ] Não há warnings no console Unity
- [ ] Não há erros no build

### Testes
- [ ] Adicionei testes que provam que meu fix funciona ou feature funciona
- [ ] Testes unitários novos e existentes passam localmente
- [ ] Testei em modo EditMode (se aplicável)
- [ ] Testei em modo PlayMode (se aplicável)
- [ ] Build local passou sem erros em todas as plataformas alvo

### Documentação
- [ ] Atualizei a documentação relevante (CLAUDE.md, ReadMe.md, etc.)
- [ ] Adicionei/atualizei comentários XML/JSDoc (se aplicável)
- [ ] Atualizei CHANGELOG.md na seção `[Unreleased]`

### CI/CD
- [ ] Workflows CI passam sem erros
- [ ] Não quebrei workflows existentes
- [ ] Testei novo workflow (se aplicável)

### Git
- [ ] Commits seguem Conventional Commits
- [ ] Branch está atualizada com develop/main
- [ ] Resolvi todos os conflitos
- [ ] Nome da branch segue convenção (feature/, fix/, etc.)

## 💭 Contexto Adicional

<!-- Adicione qualquer contexto relevante sobre o PR -->
<!-- Por exemplo: decisões de design, alternativas consideradas, etc. -->

## 🚀 Deployment Notes

<!-- Se esta mudança requer passos especiais para deploy, liste aqui -->
<!-- Por exemplo: novos secrets, configurações, migrações, etc. -->

- [ ] Nenhum passo adicional necessário
- [ ] Requer ação manual (especificar abaixo)

**Ações necessárias:**

## 📋 Checklist para Reviewers

Para reviewers do PR:

- [ ] Código está limpo e bem estruturado
- [ ] Mudanças fazem sentido no contexto do projeto
- [ ] Não há problemas óbvios de performance
- [ ] Documentação está adequada
- [ ] Testes cobrem casos importantes
- [ ] CI/CD passa sem issues
- [ ] Segui as guidelines em CONTRIBUTING.md

---

**Notas para o Reviewer:**
<!-- Adicione qualquer nota específica para quem vai revisar o PR -->
<!-- Por exemplo: "Preste atenção especial em X" ou "Dúvida sobre implementação de Y" -->
