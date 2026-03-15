👥 Integrantes do Grupo

Pietro Paranhos Wilhelm Rm561378

João Vitor Biribilli Ravelli Rm565594

Pedro de Matos Previtali Rm564184


🎯 Domínio Escolhido
O domínio selecionado para este projeto é uma Loja de Jogos (GameStore). O sistema foca no gerenciamento do catálogo de produtos (jogos), seus respectivos desenvolvedores (studios), categorização e a gestão de perfis de clientes com suas configurações personalizadas.


🔗 Resumo dos Relacionamentos

Studio 1 : N Game: Um Studio pode desenvolver múltiplos jogos, mas cada jogo é associado a um único Studio principal.

Game N : N Category: Um jogo pode pertencer a várias categorias, e uma categoria pode estar vinculada a diversos jogos (resolvido via tabela associativa).

Customer 1 : N Order: Um cliente pode realizar vários pedidos ao longo do tempo.

Game N : N : Um pedido pode conter vários jogos e um jogo pode estar em vários pedidos diferentes.