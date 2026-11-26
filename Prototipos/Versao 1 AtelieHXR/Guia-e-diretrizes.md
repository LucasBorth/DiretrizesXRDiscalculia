# Ateliê HXR — versão imersiva 0.3.0

Protótipo demonstrativo para Quest 2. A proposta trabalha a conservação da soma de 15 unidades em uma sala de aula virtual com mesa e peças manipuláveis. O aplicativo tem identificador próprio (`br.edu.prototipo.ateliehxr`) e pode coexistir com a primeira versão, Matemática HXR.

## Novidade: navegação e demonstração animada

Use **Soma anterior / Próxima soma** para percorrer livremente as 16 composições. **Ver animação / Rever animação** reúne os blocos automaticamente, com contagem por chegada; **Juntar** conserva o modo manual. É possível pausar, repetir ou mudar de soma durante a animação. As setas e a animação dispensam a prática inicial, que continua disponível no percurso manual. Consulte Novidades-0.3.0.md.

## Como experimentar

1. Comece sentado ou em pé, com espaço físico livre para os braços. A mesa é virtual: não apoie o corpo nela.
2. Use **Centralizar** olhando para a direção de trabalho. A mesa acompanha a calibração inicial da altura dos olhos; **Mesa + / −** oferece três alturas relativas.
3. Na exploração, aproxime qualquer controle de um bloco, mantenha o botão lateral pressionado, mova e solte. As peças têm volume, colisão, gravidade e podem ser empilhadas. Não é rastreamento de mãos: usa os controles Touch.
4. Como alternativa, aponte o raio e toque o gatilho para selecionar uma peça. Mova o controle e toque novamente para soltá-la. Essa opção permite uso com uma mão e sem alcançar diretamente as peças.
5. Depois de pegar e soltar uma peça, escolha **Preparar grupos**. A primeira divisão é 15 em A e 0 em B. É possível reorganizar os grupos antes de iniciar a soma.
6. Selecione **Juntar**. Leve cada bloco à bandeja **JUNTOS**. Os encaixes são tolerantes; a quantidade só muda após a soltura aceita. A expressão completa aparece apenas quando as 15 peças foram reunidas.
7. Escolha **Outra divisão**. Passe exatamente uma peça de A para B. Depois selecione **Juntar** e reúna novamente as peças. A expressão anterior continua visível para comparação. Ao chegar a 0 + 15, **Recomeçar** reinicia a sequência.
8. **Organizar** recupera posições; no resultado, **Separar de novo** permite rever a composição. **Ajuda** explica os controles. **Pausar** interrompe a manipulação e os efeitos até selecionar **Continuar**.

No editor sem headset, use o mouse para arrastar peças sobre a mesa e clicar nos botões. `R` recentraliza. Essa alternativa serve à inspeção, mas não valida tato, rastreamento e conforto em VR.

## Experiência visual e sensorial

A sala contém janela, plantas, livros, bancos, luminária e tapete, construídos com geometria simples. A área central mantém bandejas e instruções em posições previsíveis. O ambiente não usa locomoção artificial, câmera animada ou música contínua.

Cada peça vale uma unidade e recebe numeral **1**, uma marca superior e identificação **A/B**. Azul e âmbar complementam os rótulos. Ao juntar, a cor e o rótulo da origem permanecem, permitindo observar as partes no todo. A contagem das bandejas complementa a manipulação.

Os sons foram sintetizados para esta versão: sinais curtos de pegar, encaixar, confirmar e concluir. A vibração dos controles informa pegar e encaixar. Pequenos realces animados marcam encaixes, sem mover a câmera. Os controles permitem som desligado/baixo/médio, vibração desligada/leve/média e cenário simples/completo. O modo simples remove decoração e os realces animados. As preferências de som, vibração, cenário e texto são salvas localmente no dispositivo, sem identificar pessoas.

## Relação com as Diretrizes HXR

Referência: **Diretrizes HXR**, documento fornecido pelo pesquisador. “Aplicada” abaixo descreve mecanismos presentes no protótipo; não significa validação clínica, conformidade integral com WCAG ou comprovação de benefício pedagógico.

| Diretriz | Aplicação nesta versão | Limite |
|---|---|---|
| HXR_D01 — progressão por habilidades-base | No percurso manual, pegar/soltar antecede a organização. As setas e a animação permitem iniciar uma demonstração direta; o treino é opcional. Há repetição sem tempo limite. | Parcial: pegar/soltar evidencia familiarização, não domínio matemático; não há avaliação adaptativa de pré-requisitos. |
| HXR_D02 — representação concreta e multimodal | Quinze unidades manipuláveis, numeral por peça, contagens, composição espacial, som e vibração opcionais. | Não inclui narração de números nem audiodescrição. |
| HXR_D03 — do lúdico ao formal | Exploração com empilhamento; composição dos grupos; reunião manual ou animada por escolha; expressão completa após reunir. | A compreensão conceitual precisa ser observada por um mediador; a execução motora não a comprova. |
| HXR_D04 — carga cognitiva e foco | Uma instrução principal por etapa; cenário estático; sem música ou cronômetro; pausa; cenário reduzido. | A carga deve ser avaliada com público-alvo e especialistas. |
| HXR_D05 — instruções curtas e sequenciais | Mensagens de uma ação por etapa, alvos identificados e botão Ajuda com os dois modos de pegar. | Parcial: sem voz, sem tutorial animado completo e sem validação de linguagem por faixa etária. |
| HXR_D06 — personalização | Som e vibração em níveis, texto ampliado, cenário reduzido e ajuste de altura da mesa; preferências sensoriais locais persistentes. | Parcial: não há perfis clínicos, adaptação por idade nem configuração individual identificada. |
| HXR_D07 — feedback não punitivo | Soltura inválida devolve a peça com orientação neutra; quedas não eliminam peças; som breve de conclusão; sem ranking, punição ou perdas. | O efeito emocional não foi validado. |
| HXR_D10 — segurança, conforto e robustez | Experiência estacionária, câmera acompanhando cabeça, recentralização voluntária, pausa, alcance ajustável e recuperação de peças. | Conforto, desempenho sustentado e segurança precisam de teste no Quest; 72 fps é alvo configurado, não medição. |
| HXR_D11 — interação simples e alternativas | Pegar próximo com botão lateral ou selecionar à distância por dois toques de gatilho; qualquer controle; encaixes tolerantes. | Não inclui seleção pelo olhar, rastreamento de mãos nem comando remoto do mediador. |
| HXR_D12 — legibilidade e consistência | Texto claro em quadro escuro, numerais ampliáveis, bandejas estáveis, A/B e numeral nas peças além da cor. | Legibilidade real deve ser conferida no headset; não foi auditado todo o conjunto WCAG. |
| HXR_D13 — diferenciação perceptiva | A/B escrito nas bandejas e blocos diferencia a origem sem depender só de azul/âmbar; operador e igualdade aparecem separados. | Parcial: não trata todo o conjunto de confusões entre numerais semelhantes. |
| HXR_D14 — orientação espacial | Mesa, quadro e bandejas em posições constantes; tapete e cenário como referências; recentralização explícita. | Não há navegação entre ambientes nesta versão. |
| HXR_D17 — treinamento e adoção | Este guia, etapa de exploração e simulação por mouse apoiam familiarização antes da demonstração. | Parcial: não substitui capacitação do profissional ou protocolo de uso. |
| HXR_D18 — previsibilidade e controle | Início, união e próxima divisão dependem do usuário; resultado permanece até nova ação; pausa e repetição. | O encaixe e a recuperação de quedas são automáticos, com retorno visual/textual; as setas retornam a composições anteriores, mas não existe histórico completo de cada manipulação. |
| HXR_D19 — multissensorialidade controlável | Som e vibração ligados a ações concretas, reguláveis e opcionais; realce suave desligável; informação visual preservada. | Eficácia sensorial e tolerância individual exigem avaliação; a vibração depende do suporte do controle. |
| HXR_D20 — pedido de ajuda | Ajuda disponível nas etapas, instruções dos controles e pausa com convite a pedir apoio ao mediador. | Parcial: não envia alerta a painel externo nem registra pedidos. |

Não reivindicadas: **D08** (métricas e acompanhamento longitudinal), **D09** (painel profissional externo), **D15** (transferência funcional para situações cotidianas) e **D16** (continuidade clínica/família/escola). A decoração de sala de aula, isoladamente, não implementa D15.

## Comparação para a pesquisa

Mantenha a primeira versão como condição anterior e esta como condição imersiva. Nesta nova versão mudam simultaneamente manipulação, ambiente, sons e vibração. Uma diferença observada entre versões não permite atribuir o efeito a uma diretriz específica. O relatório deve identificar essa alteração conjunta e registrar como som, vibração e cenário foram configurados em cada sessão.

Não há coleta de dados de participantes ou avaliação clínica nesta versão. A experiência não é instrumento diagnóstico nem demonstra, por si só, eficácia para discalculia.

## Verificação no Quest antes de demonstrar

- Abrir Ateliê HXR e verificar mesa à frente, ajuste de altura e recentralização.
- Pegar, mover, girar, empilhar e soltar com cada controle; testar a alternativa por gatilho.
- Desligar som e vibração separadamente e confirmar que a tarefa continua compreensível.
- Comparar cenário completo/simples e texto normal/ampliado.
- Soltar fora das bandejas e fora da mesa: nenhuma das 15 peças deve desaparecer.
- Juntar todas as peças, verificar resultado 15, transferir uma unidade e repetir.
- Pausar segurando uma peça e retirar/recolocar o headset; confirmar recuperação previsível.
- Observar legibilidade, alcance e fluidez; interromper se houver desconforto.

As evidências de compilação e testes do editor acompanham a entrega. Nenhum teste de editor substitui a validação física de áudio, vibração, controles e conforto no Quest.

## Evidências desta entrega

- Verificação do modelo: todas as 16 divisões, conservação, coleta idempotente e rejeição de índices e zonas inválidos.
- Play Mode: atualização de frames, repouso físico dos blocos sobre a mesa, pausa, recuperação de queda, posse exclusiva de uma peça e sequência completa via soltura programática.
- Capturas de exploração, composição e resultado revisadas no editor.
- A movimentação física da cabeça e dos controles é acompanhada em espaço 3D. Não há joystick de locomoção ou teleportação: a atividade foi concebida para uso estacionário ou movimento físico dentro da área segura do Quest.
