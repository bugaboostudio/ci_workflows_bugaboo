#!/bin/bash
# Setup script for Bugaboo Studio Unity Template
# Este script automatiza os primeiros passos de configuração do template

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
print_header() {
    echo -e "${BLUE}"
    echo "╔═══════════════════════════════════════════════════╗"
    echo "║   Bugaboo Studio Unity Template Setup 🚀         ║"
    echo "║   Version 1.0.0                                   ║"
    echo "╚═══════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

print_step() {
    echo -e "${GREEN}[✓] $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}[!] $1${NC}"
}

print_error() {
    echo -e "${RED}[✗] $1${NC}"
}

check_prerequisites() {
    echo -e "\n${BLUE}Verificando pré-requisitos...${NC}"
    
    # Check git
    if ! command -v git &> /dev/null; then
        print_error "Git não encontrado. Por favor, instale o Git primeiro."
        exit 1
    fi
    print_step "Git instalado"
    
    # Check gh CLI (optional)
    if command -v gh &> /dev/null; then
        print_step "GitHub CLI instalado"
        GH_INSTALLED=true
    else
        print_warning "GitHub CLI não encontrado (opcional)"
        print_warning "Instale para facilitar: brew install gh"
        GH_INSTALLED=false
    fi
}

get_project_info() {
    echo -e "\n${BLUE}Informações do Projeto${NC}"
    echo "Por favor, forneça as seguintes informações:"
    echo ""
    
    read -p "Nome do projeto (ex: meu-novo-jogo): " PROJECT_NAME
    read -p "Nome de exibição (ex: Meu Novo Jogo): " DISPLAY_NAME
    read -p "Descrição breve: " DESCRIPTION
    read -p "Nome da company (padrão: Bugaboo Studio): " COMPANY_NAME
    COMPANY_NAME=${COMPANY_NAME:-"Bugaboo Studio"}
    
    echo ""
    echo "Confirme as informações:"
    echo "  Nome do projeto: $PROJECT_NAME"
    echo "  Nome de exibição: $DISPLAY_NAME"
    echo "  Descrição: $DESCRIPTION"
    echo "  Company: $COMPANY_NAME"
    echo ""
    read -p "Está correto? (y/n): " CONFIRM
    
    if [[ $CONFIRM != "y" && $CONFIRM != "Y" ]]; then
        echo "Cancelado pelo usuário."
        exit 0
    fi
}

setup_gitflow() {
    echo -e "\n${BLUE}Configurando GitFlow...${NC}"
    
    # Check current branch
    CURRENT_BRANCH=$(git branch --show-current)
    
    if [[ $CURRENT_BRANCH != "main" ]]; then
        print_warning "Branch atual não é 'main': $CURRENT_BRANCH"
        read -p "Continuar mesmo assim? (y/n): " CONTINUE
        if [[ $CONTINUE != "y" && $CONTINUE != "Y" ]]; then
            exit 0
        fi
    fi
    
    # Create develop branch
    if git show-ref --quiet refs/heads/develop; then
        print_warning "Branch 'develop' já existe"
    else
        git checkout -b develop
        print_step "Branch 'develop' criada"
    fi
    
    # Push develop
    if $GH_INSTALLED; then
        read -p "Push branch 'develop' para origin? (y/n): " PUSH_DEV
        if [[ $PUSH_DEV == "y" || $PUSH_DEV == "Y" ]]; then
            git push -u origin develop
            print_step "Branch 'develop' enviada para origin"
        fi
    else
        print_warning "Execute manualmente: git push -u origin develop"
    fi
}

update_documentation() {
    echo -e "\n${BLUE}Atualizando documentação...${NC}"
    
    # Update ReadMe.md placeholders (basic replacement)
    if [[ -f "ReadMe.md" ]]; then
        # Backup
        cp ReadMe.md ReadMe.md.backup
        
        # Replace project name in badges/links
        # Note: Mais customização pode ser feita manualmente
        print_warning "ReadMe.md: Revise e atualize manualmente conforme necessário"
        print_warning "  - Nome do projeto"
        print_warning "  - Descrição"
        print_warning "  - Links específicos"
    fi
}

setup_env_files() {
    echo -e "\n${BLUE}Configurando arquivos de ambiente...${NC}"
    
    # Create .env.example if doesn't exist
    if [[ ! -f ".env.example" ]]; then
        cat > .env.example << 'EOF'
# Unity Configuration
UNITY_VERSION=2022.3.15f1
UNITY_LICENSE_PATH=/path/to/license

# API Keys (use placeholders)
GAME_API_KEY=your_api_key_here
ANALYTICS_API_KEY=your_analytics_key_here
EOF
        print_step ".env.example criado"
    fi
    
    # Create .env.local template
    if [[ ! -f ".env.local" ]]; then
        cp .env.example .env.local
        print_step ".env.local criado (CONFIGURE com valores reais)"
        print_warning "IMPORTANTE: Adicione valores reais em .env.local"
    fi
    
    # Check .gitignore
    if ! grep -q ".env.local" .gitignore 2>/dev/null; then
        echo ".env.local" >> .gitignore
        print_step ".env.local adicionado ao .gitignore"
    fi
}

initial_commit() {
    echo -e "\n${BLUE}Commit inicial...${NC}"
    
    read -p "Fazer commit inicial das mudanças? (y/n): " DO_COMMIT
    if [[ $DO_COMMIT == "y" || $DO_COMMIT == "Y" ]]; then
        git add .
        git commit -m "chore: initial project setup from template

- Configured GitFlow (develop branch)
- Updated documentation
- Setup environment files
- Project: $DISPLAY_NAME

Co-Authored-By: Warp <agent@warp.dev>"
        print_step "Commit inicial realizado"
        
        if $GH_INSTALLED; then
            read -p "Push para origin? (y/n): " DO_PUSH
            if [[ $DO_PUSH == "y" || $DO_PUSH == "Y" ]]; then
                git push origin develop
                print_step "Push realizado"
            fi
        fi
    fi
}

show_next_steps() {
    echo -e "\n${GREEN}╔═══════════════════════════════════════════════════╗${NC}"
    echo -e "${GREEN}║   ✅ Setup Inicial Completo!                      ║${NC}"
    echo -e "${GREEN}╚═══════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${YELLOW}Próximos Passos:${NC}"
    echo ""
    echo "1. 📝 Configure Unity License no GitHub:"
    echo "   gh workflow run activation.yml"
    echo "   (Veja TEMPLATE_SETUP.md para detalhes)"
    echo ""
    echo "2. 🎮 Abra o projeto no Unity Hub:"
    echo "   - Add project from disk"
    echo "   - Configure project settings (nome, company, etc.)"
    echo ""
    echo "3. 🔒 Configure GitHub Secrets:"
    echo "   - UNITY_LICENSE (obrigatório)"
    echo "   - Secrets de plataforma (se necessário)"
    echo ""
    echo "4. 🔧 Configure branch protection rules:"
    echo "   - Proteja 'main' e 'develop'"
    echo "   - Require PR reviews"
    echo ""
    echo "5. 📚 Leia a documentação completa:"
    echo "   - TEMPLATE_SETUP.md: Guia passo-a-passo"
    echo "   - CLAUDE.md: Documentação técnica completa"
    echo "   - SECURITY.md: Políticas de segurança"
    echo ""
    echo -e "${BLUE}Documentação:${NC}"
    echo "  https://github.com/bugaboostudio/ci_workflows_bugaboo"
    echo ""
}

# Main execution
main() {
    print_header
    check_prerequisites
    get_project_info
    setup_gitflow
    update_documentation
    setup_env_files
    initial_commit
    show_next_steps
}

# Run main
main
