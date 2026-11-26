# Matemática HXR — protótipo demonstrativo

Unity 6000.4.9f1 · Quest 2 · OpenXR · português brasileiro

## Objetivo e recorte

Demonstrar a conservação da soma: 15 + 0, 14 + 1, …, 0 + 15.
Cada passo retira uma unidade do primeiro grupo e acrescenta uma ao segundo.
O total permanece 15. O protótipo apresenta a sequência numérica da fotografia,
sem reproduzir a página comercial ou os problemas de texto laterais.

São duas condições no mesmo aplicativo, selecionáveis no menu inicial:

- **A — Somente números:** operação simbólica, botão Somar revela o resultado.
- **B — Números e blocos:** numerais e dois conjuntos de cubos, azul e laranja;
  cada cubo vale uma unidade; Somar reúne fisicamente os mesmos 15 cubos.

Os grupos têm rótulos A/B e quantidades escritas. Cor não é o único código.
Voltar e Próximo percorrem as mesmas 16 combinações nas duas condições.
O menu permite alternar de condição preservando a combinação atual; Reiniciar
retorna a 15 + 0. Não há cronômetro, pontuação, respostas classificadas, coleta de
dados pessoais, gravação de áudio ou conexão com serviço externo no protótipo.

## Testar no Mac

Abra MatematicaHXR no Unity Hub. Abra Assets/Scenes/Inicio.unity e pressione Play.
Clique nos botões da aba Game com o mouse. R centraliza o painel.
A primeira importação prepara as cenas automaticamente. Se necessário, use
**HXR > 1 - Preparar cenas e Quest 2** fora do Play (recria as três cenas geradas).

## Testar no Quest 2

1. Use **HXR > 3 - Gerar APK Quest 2** no editor, fora do Play.
2. O arquivo esperado é `Builds/MatematicaHXR-Quest2.apk`.
3. Com modo de desenvolvedor habilitado, conecte o Quest 2 ao Mac por USB e aceite
   a depuração USB dentro do headset.
4. No Meta Quest Developer Hub, selecione o dispositivo e instale o APK.
5. Abra Matemática HXR no headset (aplicativos de fontes desconhecidas).
6. Aponte com qualquer controle Touch e aperte o gatilho para selecionar.
7. Use Centralizar para posicionar o painel diante de você, Pausar para interromper
   a animação e Movimento: não para concluir as mudanças sem movimento.

O aplicativo é autônomo: depois de instalado, executa no Quest. O Mac serve para
edição, compilação e instalação. Android Build Support, SDK, NDK e JDK são usados
porque o sistema do Quest é baseado em Android. Não é necessário adicionar o
pacote completo da Meta para esta demonstração com OpenXR e controles Touch.

## Diretrizes usadas neste recorte

Referência: Diretrizes HXR, material fornecido pelo pesquisador.

| Diretriz | Aplicação e limite |
| --- | --- |
| HXR_D02 | Correspondência numeral/quantidade/espaço; 1 cubo = 1 unidade; reunião dos conjuntos. Sem narração ou manipulação livre. |
| HXR_D03 | Ação concreta acompanha os símbolos e antecede a revelação do resultado em B. Aplicação parcial: não há sequência lúdica completa. |
| HXR_D04 | Uma operação por vez; ambiente sem decoração; movimento opcional e pausável. |
| HXR_D05 | Instruções curtas para observar, somar e avançar. Sem voz nesta versão. |
| HXR_D06 | Movimento opcional e ritmo livre. Sem perfis individuais; aplicação parcial. |
| HXR_D10 | Sem locomoção artificial; painel fixo no espaço, centralização voluntária. Conforto e desempenho precisam de teste no headset. |
| HXR_D11 | Apontar e confirmar com qualquer controle; mouse para revisão no Mac. Sem gestos complexos. |
| HXR_D12 | Texto claro, fundo escuro, rótulos e posições consistentes. Legibilidade deve ser conferida no Quest 2. |
| HXR_D18 | Avanço apenas por ação explícita, retorno e repetição disponíveis. |

As condições compartilham controles básicos, legibilidade e conforto. Portanto,
A é uma **condição simbólica sem o apoio concreto selecionado**, e não ausência
absoluta de toda diretriz. A comparação é demonstrativa, não comprova eficácia
clínica ou pedagógica. O recorte não implementa as 20 diretrizes integralmente.
Não há instrumento de coleta ou protocolo experimental nesta versão.

## Verificação

**HXR > 2 - Verificar matemática** testa as 16 combinações, limites, retorno e reinício.
**HXR > 4 - Testar e capturar apresentações** executa uma verificação em Play Mode,
salva `QA-result.txt` e imagens em `../Previews`.

Antes de demonstrar com o headset, verificar: instalação e abertura, acompanhamento
da cabeça, seleção com cada controle, centralização sentado, legibilidade, soma
com 15 blocos, pausa e repetição, e estabilidade de execução. Testes no editor não
substituem a validação no dispositivo físico.
