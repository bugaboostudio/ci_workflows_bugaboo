# 🔒 Security Policy & Best Practices

Este documento descreve políticas de segurança e melhores práticas para o template Unity da Bugaboo Studio.

---

## 📋 Índice

- [Reportando Vulnerabilidades](#reportando-vulnerabilidades)
- [Gestão de Secrets](#gestão-de-secrets)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [GitHub Actions Security](#github-actions-security)
- [Unity Project Security](#unity-project-security)
- [Melhores Práticas](#melhores-práticas)

---

## 🚨 Reportando Vulnerabilidades

### Como Reportar

Se você descobrir uma vulnerabilidade de segurança, **NÃO** abra uma issue pública.

**Em vez disso:**

1. **Email**: Envie para o time de segurança da Bugaboo Studio
   - Assunto: `[SECURITY] Nome do Projeto - Descrição Breve`
   - Inclua detalhes completos da vulnerabilidade
   - Aguarde confirmação de recebimento (24-48 horas)

2. **GitHub Security Advisory** (se disponível):
   - Acesse: Repository → Security → Advisories → New draft security advisory
   - Preencha detalhes
   - Aguarde resposta do time

### O que Incluir no Report

```markdown
## Vulnerabilidade

**Tipo**: [SQL Injection / XSS / Exposed Secret / etc.]
**Severidade**: [Crítica / Alta / Média / Baixa]
**Componente Afetado**: [Workflow / Script / Asset / etc.]

## Descrição

[Descrição detalhada da vulnerabilidade]

## Passos para Reproduzir

1. 
2. 
3. 

## Impacto Potencial

[Descrição do impacto se explorado]

## Sugestão de Fix

[Se possível, sugira uma solução]
```

### Response Time

- **Críticas**: Resposta em 24 horas, fix em 7 dias
- **Altas**: Resposta em 48 horas, fix em 30 dias
- **Médias/Baixas**: Resposta em 5 dias, fix conforme prioridade

---

## 🔑 Gestão de Secrets

### GitHub Secrets

**Secrets Necessários para CI/CD:**

#### Obrigatórios
- `UNITY_LICENSE`: Conteúdo do arquivo .ulf da Unity
  - **Como obter**: Ver [TEMPLATE_SETUP.md](TEMPLATE_SETUP.md#passo-3-configurar-unity-license-para-cicd)
  - **Rotação**: Anual ou quando expirar
  - **Acesso**: Repository secrets (Actions)

#### Opcionais (por plataforma)

**Android:**
- `ANDROID_KEYSTORE_BASE64`: Keystore codificado em base64
- `ANDROID_KEYSTORE_PASS`: Senha do keystore
- `ANDROID_KEYALIAS_NAME`: Nome do alias
- `ANDROID_KEYALIAS_PASS`: Senha do alias

**iOS:**
- `IOS_CERTIFICATE_BASE64`: Certificado codificado
- `IOS_CERTIFICATE_PASSWORD`: Senha do certificado
- `IOS_PROVISIONING_PROFILE_BASE64`: Provisioning profile
- `APPLE_TEAM_ID`: ID do time Apple Developer

**AWS (se usar):**
- `AWS_ACCESS_KEY_ID`: Access key da AWS
- `AWS_SECRET_ACCESS_KEY`: Secret key da AWS
- `AWS_REGION`: Região AWS (ex: us-east-1)

### ❌ NUNCA Faça

```bash
# ❌ NUNCA commite secrets diretamente
API_KEY="sk-1234567890abcdef"

# ❌ NUNCA logue secrets
echo "API Key: $API_KEY"
console.log(`Secret: ${process.env.SECRET}`)

# ❌ NUNCA inclua secrets em mensagens de commit
git commit -m "Add API key: sk-1234567890abcdef"

# ❌ NUNCA adicione secrets em PRs ou Issues
Issue: "Getting error with key: sk-1234567890abcdef"
```

### ✅ Faça

```bash
# ✅ Use secrets de forma segura
export API_KEY=$(cat secret.txt)
curl -H "Authorization: Bearer $API_KEY" https://api.example.com

# ✅ Referencie sem expor
echo "API key is configured"

# ✅ Use .env.local para desenvolvimento
# Adicione ao .gitignore
echo ".env.local" >> .gitignore
```

---

## 🌍 Variáveis de Ambiente

### Estrutura Recomendada

```
projeto/
├── .env.example        # Template público (SEM valores reais)
├── .env.local          # Valores locais (NÃO commitado)
├── .env.development    # Desenvolvimento (se necessário)
├── .env.production     # Produção (apenas em CI/CD)
└── .gitignore          # Deve incluir .env.local
```

### .env.example (Template)

```bash
# Unity Configuration
UNITY_VERSION=2022.3.15f1
UNITY_LICENSE_PATH=/path/to/license

# API Keys (use placeholders)
GAME_API_KEY=your_api_key_here
ANALYTICS_API_KEY=your_analytics_key_here

# AWS Configuration (exemplo)
AWS_REGION=us-east-1
AWS_S3_BUCKET=your-bucket-name

# Playfab (exemplo)
PLAYFAB_TITLE_ID=your_title_id
PLAYFAB_SECRET_KEY=your_secret_key

# Photon (exemplo)
PHOTON_APP_ID=your_app_id
```

### .env.local (Local Development)

```bash
# Este arquivo NÃO deve ser commitado
# Copie de .env.example e preencha com valores reais

UNITY_LICENSE_PATH=/Users/seu-usuario/.unity/license.ulf
GAME_API_KEY=sk-live-1234567890abcdef
ANALYTICS_API_KEY=ak-test-9876543210fedcba
PLAYFAB_TITLE_ID=ABC123
PLAYFAB_SECRET_KEY=SECRETO123
```

### .gitignore Essencial

```gitignore
# Secrets e Configurações Locais
.env.local
.env.*.local
*.key
*.pem
*.p12
*.cer
*.mobileprovision
*.keystore

# Unity License Files
*.alf
*.ulf

# Credentials
credentials.json
secrets.yml
config.local.*

# AWS
.aws/
aws-credentials.txt

# Logs que podem conter informações sensíveis
*.log
logs/
```

---

## 🔐 GitHub Actions Security

### Melhores Práticas em Workflows

```yaml
# ✅ BOM: Usar secrets corretamente
- name: Deploy
  env:
    API_KEY: ${{ secrets.API_KEY }}
  run: |
    # Secret está disponível mas não exposto em logs
    deploy.sh

# ❌ RUIM: Expor secret em logs
- name: Deploy
  run: |
    echo "Using API key: ${{ secrets.API_KEY }}"  # ❌ NUNCA!
```

### Permissões Mínimas

```yaml
# Definir permissões explícitas
permissions:
  contents: read        # Apenas leitura do código
  pull-requests: write  # Necessário para comentários
  actions: read         # Leitura de workflows
```

### Validação de Inputs

```yaml
on:
  workflow_dispatch:
    inputs:
      environment:
        type: choice
        options:
          - development
          - staging
          - production
        required: true

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Validate input
        run: |
          # Validar que environment é permitido
          if [[ ! "${{ inputs.environment }}" =~ ^(development|staging|production)$ ]]; then
            echo "❌ Invalid environment"
            exit 1
          fi
```

### Evitar Command Injection

```yaml
# ❌ VULNERÁVEL a command injection
- name: Deploy
  run: |
    deploy.sh ${{ github.event.issue.title }}

# ✅ SEGURO: usar variável de ambiente
- name: Deploy
  env:
    ISSUE_TITLE: ${{ github.event.issue.title }}
  run: |
    deploy.sh "$ISSUE_TITLE"
```

---

## 🎮 Unity Project Security

### PlayerPrefs Security

```csharp
// ❌ NUNCA armazene secrets em PlayerPrefs
PlayerPrefs.SetString("API_KEY", "sk-1234567890");  // ❌

// ✅ Use sistema de keychain/keystore do OS
#if UNITY_IOS
    // iOS Keychain
#elif UNITY_ANDROID
    // Android KeyStore
#endif
```

### Build Security

```csharp
// Remover logs de debug em production builds
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
    Debug.unityLogger.logEnabled = false;
#endif

// Ofuscar código sensível
[System.Runtime.CompilerServices.MethodImpl(
    System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
private string GetApiKey()
{
    // Implementação ofuscada
    return DecryptFromResources();
}
```

### ScriptableObjects com Secrets

```csharp
// ❌ NUNCA commite ScriptableObjects com secrets
[CreateAssetMenu(fileName = "APIConfig", menuName = "Config/API")]
public class APIConfig : ScriptableObject
{
    public string apiKey = "sk-1234567890";  // ❌ Commitado no repo!
}

// ✅ Use arquivos locais não-commitados
[CreateAssetMenu(fileName = "APIConfig", menuName = "Config/API")]
public class APIConfig : ScriptableObject
{
    public string apiKey;  // Vazio no repo
    
    // Carregar de arquivo local em runtime
    public void LoadFromLocal()
    {
        // Ler de arquivo .env.local ou similar
    }
}
```

---

## 🛡️ Melhores Práticas

### 1. Princípio do Menor Privilégio

- Conceda apenas as permissões **mínimas necessárias**
- GitHub teams: defina roles apropriados (read, write, admin)
- Secrets: compartilhe apenas com quem precisa

### 2. Rotação de Secrets

| Secret | Frequência de Rotação |
|--------|----------------------|
| Unity License | Anual ou quando expirar |
| API Keys (produção) | A cada 90 dias |
| Keystores Android | Apenas se comprometido |
| Certificados iOS | Anual (quando expirar) |
| AWS Keys | A cada 90 dias |
| Database Passwords | A cada 30-60 dias |

### 3. Auditoria

```bash
# Verificar se há secrets commitados acidentalmente
git log -p | grep -i "password\|secret\|key" | grep -v ".md"

# Usar ferramentas de scanning
# - TruffleHog: https://github.com/trufflesecurity/trufflehog
# - GitLeaks: https://github.com/zricethezav/gitleaks
# - GitHub Secret Scanning (se disponível)
```

### 4. Incident Response

**Se um secret foi exposto:**

1. ✅ **Revogar imediatamente** o secret comprometido
2. ✅ **Gerar novo** secret
3. ✅ **Atualizar** em todos os lugares necessários
4. ✅ **Investigar** uso indevido (logs, analytics)
5. ✅ **Documentar** o incidente
6. ✅ **Revisar** processos para evitar recorrência

### 5. Code Review Security Checklist

Durante code review, verificar:

- [ ] Nenhum secret hardcoded no código
- [ ] `.env.local` está no `.gitignore`
- [ ] Secrets usados via environment variables
- [ ] Logs não expõem informações sensíveis
- [ ] Workflows GitHub Actions seguem best practices
- [ ] Permissões de workflows são mínimas
- [ ] Inputs de usuários são validados (XSS, injection)

---

## 📚 Recursos Adicionais

### Documentação Oficial

- [GitHub Actions Security](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [GitHub Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Unity Build Settings Security](https://docs.unity3d.com/Manual/built-in-custom-build-options.html)

### Ferramentas de Scanning

- [TruffleHog](https://github.com/trufflesecurity/trufflehog) - Scan git history for secrets
- [GitLeaks](https://github.com/zricethezav/gitleaks) - Secret detection
- [Semgrep](https://semgrep.dev/) - Static analysis security scanning

### Standards e Compliance

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE/SANS Top 25](https://www.sans.org/top25-software-errors/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

---

## 📞 Contato de Segurança

**Security Team**: [Adicionar email/contato]

**Response Time SLA:**
- Crítico: 24 horas
- Alto: 48 horas
- Médio: 5 dias úteis
- Baixo: 10 dias úteis

---

**Última Atualização**: Janeiro 2026
**Versão**: 1.0.0
**Maintainer**: Bugaboo Studio Security Team

---

<div align="center">

🔒 **Segurança é responsabilidade de todos!** 🔒

*Se você ver algo, reporte algo.*

</div>
