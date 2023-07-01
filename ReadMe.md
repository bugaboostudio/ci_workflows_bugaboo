# OBS: [30/06/2023] estamos realizando um teste administrando o fluxo de arte dentro da unity com esse repositório para que ele seja usado como referência para os próximos 

![Workflow demo](https://user-images.githubusercontent.com/1744957/125079869-022e6300-e0bc-11eb-85f0-291a08a65507.png)

## Intro
 - Aqui na bugaboo studio estamos usando o github actions para alguns processos de CI nossos usando as boas práticas do projeto GameCI e uma série de especialistas. Caso tenha dúvida pode falar comigo (Tiago) para ajudar nesse processo de configuração.

# Leia antes de configurar...


 - **Por favor, leia a documentação completa em ** https://game.ci/docs/github/getting-started

## Links

 - [Game.ci - Primeiros Passos](https://game.ci/docs/github/getting-started)
 - [Github "Unity Actions" Project](https://github.com/game-ci/unity-actions)


## Passos para configurar o fluxo de trabalho do Game CI para um novo repositório unity.

### Criando um projeto Unity.

 - Digite na pasta raiz do projeto: `git init`
 - Adicione seu git ao nosso github `git remote add origin ...`
 - Pegue arquivo .gitignore de [gitignore.io](https://www.toptal.com/developers/gitignore/api/unity)
 - git add and commit and push
 - Create your actions workflow: `.github/workflows/main.yml`
 - git add and commit and push


### Crie uma Licença
 - [DOCUMENTATION](https://game.ci/docs/github/activation)
 - Coloque para rodar o workflow activation: `.github/workflows/activation.yml` ver o documento acima.
 - Check that your workflow passed.
 - Download `Manual Activation File` artifact.
 - Extract zip.
 - Faça o upload do arquivo para: [license generate](https://license.unity3d.com/manual)
 - Download `Unity_*.ulf` file


### faça um update da variável 

 - Adicione uma variável secreta para seu repositório.
   - `UNITY_LICENSE` - The content of the file you just downloaded


### Customise your workflow to run tests.
 - [DOCUMENTATION](https://game.ci/docs/github/test-runner)
 - Edit main.yml.
 - Add edit/play/all test jobs to you workflow.
 - git add and commit and push

### Customise your workflow to build games.
 - [DOCUMENTATION](https://game.ci/docs/github/builder)
 - Edit main.yml.
 - Add jobs for your build targets to the workflow.
 - (optional) Make build jobs dependent on tests passing.
 - git add and commit and push
 - Download Standalone build or Android build to test.

### Customise your workflow to deploy games.
 - Edit main.yml.
 - Add a job for deploying to github pages.
 - Make deploy job dependent on the webgl build passing.
 - git add and commit and push
 - Go to github pages settings and select which branch has your pages webgl deployment (eg. `gh-pages`)
 - Go to `https://[your-github-name].github.io/[your-repo-name]` to test your web build.
