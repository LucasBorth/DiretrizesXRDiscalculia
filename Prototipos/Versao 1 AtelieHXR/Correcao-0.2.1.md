# Correção 0.2.1 — inicialização no Android

A compilação 0.2.0 omitiu SphereCollider e CapsuleCollider do registro nativo do Unity. A inicialização cria uma esfera para representar o controle antes de construir a sala e acessava seu Collider sem verificar a existência. Essa combinação é compatível com a interrupção da inicialização e o fundo vazio relatado. Não foi possível confirmar o stack trace no aparelho, pois esta correção foi feita sem conexão ao Quest, conforme solicitado.

## Alterações

- Preservação explícita de SphereCollider, CapsuleCollider, BoxCollider, MeshFilter e MeshRenderer na compilação Android.
- Verificação de existência do collider antes de desativá-lo em objetos decorativos.
- A atividade só é marcada como pronta depois de criar sala, mesa, quadro, blocos e áudio; a atualização não executa sobre um estado parcialmente inicializado.
- Registro da etapa de inicialização e mensagem visível caso ocorra uma nova exceção após a criação da câmera.
- Versão 0.2.1, código Android 3, mesmo pacote br.edu.prototipo.ateliehxr para atualizar a instalação existente.

## Instalação

Instale AtelieHXR-Quest2-0.2.1.apk sobre a versão anterior e abra Ateliê HXR. A primeira tela da atividade deve mostrar sala, mesa, 15 blocos e instruções. Não há música contínua: os sons são feedback das ações.

Os testes no editor passaram novamente. A presença dos componentes na saída nativa Android e a assinatura do APK são verificadas separadamente. A execução física no Quest permanece pendente.

Referência técnica: [Unity — GameObject.CreatePrimitive](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html).
