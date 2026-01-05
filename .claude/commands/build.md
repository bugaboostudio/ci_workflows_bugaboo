---
description: Preparar e executar build para plataforma específica
---

Você deve ajudar o desenvolvedor a preparar e executar build do projeto.

## Processo

1. **Confirmar plataforma**:
   - Perguntar: "Para qual plataforma deseja buildar?"
     - Windows (StandaloneWindows64)
     - macOS (StandaloneOSX)
     - Linux (StandaloneLinux64)
     - Android
     - iOS
     - WebGL
     - Todas (via CI/CD)

2. **Pre-build checklist**:
   - [ ] Todos os testes passam
   - [ ] Sem warnings no console
   - [ ] Sem missing references
   - [ ] Versão atualizada em ProjectSettings
   - [ ] Build settings configurados

3. **Local Build**:

   **Opção A - Via Unity Editor**:
   - File > Build Settings
   - Selecionar plataforma
   - Configurar opções
   - Build

   **Opção B - Via Unity CLI** (recomendado para automation):
   ```bash
   # Windows
   unity-editor -quit -batchmode -projectPath . -buildTarget Win64 -buildWindows64Player builds/Windows/game.exe

   # Android
   unity-editor -quit -batchmode -projectPath . -buildTarget Android -executeMethod BuildScript.BuildAndroid

   # WebGL
   unity-editor -quit -batchmode -projectPath . -buildTarget WebGL -executeMethod BuildScript.BuildWebGL
   ```

4. **CI/CD Build**:

   **Via GitHub Actions**:
   ```bash
   # Trigger main workflow
   gh workflow run main.yml

   # Ou apenas para plataforma específica
   gh workflow run build-windows.yml
   ```

5. **Build Settings por Plataforma**:

   **Android**:
   - Minimum API: 21 (Android 5.0)
   - Target API: Latest
   - Scripting Backend: IL2CPP
   - Architecture: ARM64
   - Keystore configurado

   **iOS**:
   - Minimum iOS: 11.0
   - Scripting Backend: IL2CPP
   - Architecture: ARM64
   - Provisioning profile configurado

   **WebGL**:
   - Compression: Brotli
   - Memory Size: 256-512MB
   - Exception Support: Explicitly Thrown

6. **Post-build verification**:
   - [ ] Build completa sem erros
   - [ ] Tamanho de build razoável
   - [ ] Executar e testar build
   - [ ] Verificar performance

7. **Upload/Deploy**:
   - Onde fazer upload do build?
   - Steam, Play Store, App Store, Itch.io, etc.

Use o skill unity-dev para orientação sobre build settings por plataforma.
