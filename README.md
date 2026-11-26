# Diretrizes de Acessibilidade no Desenvolvimento de Experiências XR para Pessoas com Discalculia

## Sobre a pesquisa

Este repositório reúne uma seleção de artefatos da pesquisa de mestrado de **Lucas Henrique Alves Borth**, desenvolvida no **Programa de Pós-Graduação em Ciência da Computação da Universidade Federal de Mato Grosso do Sul (UFMS), Faculdade de Computação (FACOM)**, sob orientação da **Profa. Dra. Débora Maria Barroso Paiva** e coorientação do **Prof. Dr. Awdren de Lima Fontão**.

A pesquisa teve como objetivo elicitar, sistematizar e avaliar diretrizes de acessibilidade com alinhamento pedagógico para apoiar o desenvolvimento de experiências de Realidade Estendida (XR) destinadas a pessoas com discalculia. O trabalho resultou em **20 diretrizes**, acompanhadas de recomendações práticas, exemplos, critérios de verificação e vínculos com as fontes utilizadas em sua elaboração.

O propósito deste repositório é documentar o percurso da pesquisa, permitir a consulta dos produtos elaborados e tornar rastreáveis as relações entre os registros selecionados, a análise temática e as diretrizes. O acervo é uma seleção preparada para compartilhamento; não corresponde à divulgação integral dos dados brutos coletados.

## Percurso metodológico

1. Revisão sistemática sobre tecnologias imersivas na educação inclusiva.
2. Revisão focal exploratória rápida para aprofundar o recorte relacionado à discalculia.
3. Investigação com profissionais da psicopedagogia, incluindo questionários, entrevistas, discussão de atividades e seleção contextualizada de recomendações WCAG.
4. Análise temática com apoio de modelos de linguagem e revisão humana: **330 códigos iniciais, 298 selecionados e 13 temas**.
5. Elaboração de 20 hipóteses de diretrizes e refinamento colaborativo com cinco profissionais de diferentes áreas do desenvolvimento XR.
6. Avaliação por seis pesquisadores da área de acessibilidade, com questões fechadas em escala de quatro pontos e contribuições qualitativas.
7. Demonstração da aplicação de parte das diretrizes em protótipos de realidade virtual para o Meta Quest 2.

## Organização dos materiais

| Pasta | Conteúdo e finalidade |
| --- | --- |
| [Revisoes Literatura](Revisoes%20Literatura) | Resultados das questões de pesquisa das revisões, reunidos em `Questoes e Respostas.pdf`. |
| [Psicopedagogas](Psicopedagogas) | Guia de entrevista com anotações e planilha da seleção de recomendações WCAG. |
| [Analise Temática](Analise%20Tema%CC%81tica) | Planilha com etapas de codificação, seleção, organização e refinamento dos temas, relações com resultados das revisões e hipóteses de diretrizes. A subpasta `Prompts` reúne os arquivos E1 a E5. |
| [Diretrizes](Diretrizes) | Documentos individuais HXR_D01 a HXR_D20 e o documento que reúne o conjunto, na subpasta `Mesclado`. |
| [Qustionário Pesquisadores](Qustiona%CC%81rio%20Pesquisadores) | Instrumento de avaliação e planilha de respostas dos pesquisadores. O nome da pasta foi preservado conforme a organização do acervo. |
| [Metodologia](Metodologia) | Diagrama metodológico em PNG, SVG e arquivo XML BPMN denominado `Diagrama`. |
| [Prototipos](Prototipos) | Aplicativos APK, projeto Unity, arquivos compactados de projeto e documentação de desenvolvimento e demonstração. |

## Por onde começar

- Consulte o [documento completo das diretrizes](Diretrizes/Mesclado/Diretrizes%20HXR.pdf).
- Observe o [diagrama metodológico](Metodologia/Diagrama.png) para compreender a sequência das etapas.
- Examine a [planilha da análise temática](Analise%20Tema%CC%81tica/analise%20tematica%20validado%20profissionais%20xr.xlsx) para acompanhar os produtos intermediários.
- Consulte o [questionário dos pesquisadores](Qustiona%CC%81rio%20Pesquisadores/Questionario.pdf) para conhecer os quesitos empregados na avaliação.
- Explore a documentação dos protótipos para identificar exemplos de implementação.

## Como acompanhar a rastreabilidade

O percurso analítico pode ser consultado na seguinte sequência:

**Registros selecionados → códigos → temas → relações com as revisões e WCAG → hipóteses de diretrizes → refinamento → avaliação → conjunto consolidado → demonstração.**

Na planilha da análise temática, as abas documentam saídas da extração, códigos selecionados, temas gerados, possíveis lacunas, temas refinados e relações com resultados das revisões. A aba `Hipótese de Diretrizes` registra uma etapa do processo; sua denominação não deve ser confundida com a versão consolidada apresentada em `Diretrizes/Mesclado`.

Nos documentos das diretrizes:

- **HXR_D01 a HXR_D20** identificam as diretrizes.
- **Código Base** remete aos identificadores dos registros codificados. Os números não correspondem necessariamente a uma sequência contínua após a seleção.
- **Temas Base** relaciona a diretriz aos temas T1 a T13.
- **Resultados da Revisão Relacionados** remete aos resultados identificados nas revisões bibliográficas.
- **WCAG Relacionados** registra critérios considerados na construção da recomendação; essa relação não declara conformidade de uma experiência XR com as WCAG.

Os documentos individuais e o documento mesclado devem ser mantidos sincronizados em futuras atualizações. Perguntas de validação e observações preservadas nos registros documentam o percurso de construção e podem mencionar verificações futuras. Não representam, por si, evidências de testes já realizados.

## Protótipos demonstrativos

Os protótipos exploram a conservação do total em diferentes composições de quinze unidades: `15 + 0`, `14 + 1`, `13 + 2`, até `0 + 15`. Ao retirar uma unidade da primeira parcela e acrescentá-la à segunda, o resultado permanece igual a quinze.

**A nomenclatura acadêmica das versões deve ser distinguida dos nomes atuais das pastas:**

| Versão na dissertação | Aplicação | Localização atual |
| --- | --- | --- |
| **Versão 1 — painel frontal** | Matemática HXR: apresentação A somente com números e apresentação B com números, blocos e reunião animada. | [Versao 2 MatematicaHXR](Prototipos/Versao%202%20MatematicaHXR) |
| **Versão 2 — ambiente tridimensional** | Ateliê HXR: mesa, blocos manipuláveis, reunião animada, navegação entre somas e recursos sonoros e hápticos ajustáveis. | [Versao 1 AtelieHXR](Prototipos/Versao%201%20AtelieHXR) |

## Ética, confidencialidade e seleção dos dados

A investigação com participantes foi conduzida no âmbito do projeto submetido ao CEP/UFMS. Para preparar esta seleção de artefatos, o pesquisador realizou procedimentos de anonimização e supressão de informações, buscando preservar a confidencialidade dos participantes e de terceiros mencionados nos registros.

A disponibilidade de arquivos neste repositório não autoriza tentativas de reidentificação, associação de relatos a pessoas específicas ou cruzamento de informações para reconstruir identidades. Relatos e documentos devem ser interpretados em seu contexto de pesquisa, sem atribuir a todos os indivíduos com discalculia as características observadas nos registros.

Dados brutos identificáveis e documentos de consentimento assinados não integram o escopo de divulgação deste acervo. A ausência de determinados registros ou trechos pode decorrer da proteção do sigilo. O compartilhamento de informações adicionais depende das condições de consentimento, das autorizações aplicáveis e da avaliação do pesquisador responsável.

## Citação e atribuição

Ao utilizar as diretrizes ou outros artefatos, mencione a pesquisa e identifique os arquivos e a versão consultados. Referência descritiva do trabalho:

> BORTH, Lucas Henrique Alves. *Diretrizes de Acessibilidade no Desenvolvimento de Experiências XR para Pessoas com Discalculia*. Universidade Federal de Mato Grosso do Sul, Campo Grande, 2026.

Quando houver uma versão arquivada ou um identificador persistente, inclua-o na citação dos artefatos. A disponibilização pública não implica uma licença irrestrita de reutilização. Recursos, bibliotecas e materiais de terceiros permanecem sujeitos às respectivas licenças e atribuições, delimitadas neste repositório pela CC BY 4.0, confira o arquivo LICENSE.

## Créditos e apoio

A pesquisa contou com contribuições de profissionais da psicopedagogia, profissionais de desenvolvimento XR e pesquisadores de acessibilidade, além de revisões de membros do LEDES/FACOM. Registra-se o apoio da UFMS, a colaboração da UFOP e o apoio financeiro da CAPES, conforme os agradecimentos da dissertação.

