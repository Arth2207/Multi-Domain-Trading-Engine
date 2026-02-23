# Multi-Domain-Trading-Engine
📈 Multi-Domain Trading Engine (MDTE)
Um motor de cruzamento de ordens (Order Matching Engine) de alta performance, construído em C# (.NET 8) sob os princípios do Domain-Driven Design (DDD) e Clean Architecture.

💻 Sobre o Projeto
O Multi-Domain Trading Engine não é um sistema de cadastro tradicional. Ele é um simulador de economia virtual e um motor financeiro projetado para lidar com alta concorrência e processamento em memória.

O sistema orquestra a compra e venda de ativos entre dezenas de empresas (Agentes de Mercado) distribuídas em diferentes Tiers hierárquicos, roteando as ordens através de múltiplos mercados paralelos, cada um com suas próprias leis econômicas e regras de validação.

O objetivo principal deste projeto é demonstrar o domínio sobre estruturas de dados complexas, resolução de Race Conditions em ambientes de alta concorrência e a separação estrita entre o estado em memória (RAM) e a persistência transacional ACID (Banco de Dados).

⚙️ Principais Funcionalidades (Core)
In-Memory Order Book: O coração do sistema. As ordens ativas são mantidas e cruzadas em listas na memória RAM garantindo latência mínima, obedecendo rigorosamente à regra de Price-Time Priority (Prioridade de Preço e Tempo).

Multi-Market Routing (Padrão Strategy): O sistema roteia ordens através de continentes lógicos distintos sem acoplamento de código:

🛡️ Safe Zone: Mercado regulado com bandas de preço (teto/piso) e cobrança de impostos.

⚔️ War Zone: Mercado anárquico de livre concorrência e alta volatilidade.

🤫 Secret Market: Mercado VIP restrito a empresas de Tier A e S.

Liquidação Atômica (Settlement): Após o Match na memória, o sistema executa a transferência de ativos e moedas no banco de dados através de transações seguras, prevenindo Double-Spending.

Domain-Driven Design (DDD): Entidades ricas e encapsuladas. O saldo da carteira (Wallet) e o estoque (Inventory) são imutáveis externamente, garantindo a integridade absoluta das regras de negócio.

🛠️ Stack Tecnológica
Linguagem: C# (.NET 8)

Persistência de Dados: Entity Framework Core (EF Core)

Banco de Dados: SQLite (Fase de Desenvolvimento/MVP)

Arquitetura: Clean Architecture & Padrões de Projeto (Strategy, Result Pattern).
