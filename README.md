# 🏭 SIAI - Sistema Integrado de Alerta Industrial

O **SIAI** é uma solução de monitoramento de telemetria e gestão de alertas críticos para ambientes industriais. O projeto utiliza uma arquitetura baseada em microserviços utilizando persistência distribuída (**SQL Server** e **SQLite**) e comunicação via **API REST**.

## 🚀 Funcionalidades

* **Monitoramento em Tempo Real:** Visualização de alertas gerados automaticamente por sensores.
* **Processamento Inteligente:** Geração automática de alertas críticos quando leituras de sensores excedem limites de segurança (ex: > 90°C).
* **Gestão Manual:** Interface para operadores registrarem ocorrências e avisos administrativos.
* **Persistência Híbrida (Offline First):**
    * **SQL Server:** Banco centralizado para auditoria e relatórios globais.
    * **SQLite Local:** Cache de resiliência na interface WPF para funcionamento em caso de queda da rede.
* **Documentação Autogerada:** API totalmente documentada via Swagger com comentários técnicos XML.

## 🏗️ Arquitetura do Sistema

O sistema é composto por três camadas principais:

1.  **Worker (Simulador):** Simula a telemetria industrial enviando dados para o processamento centralizado.
2.  **API de Processamento:** Camada lógica que recebe dados, aplica regras de negócio e persiste no banco central.
3.  **Interface WPF (MVVM):** Painel central do operador para monitoramento e ações manuais.

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET 6/8)
* **Frontend:** WPF (Windows Presentation Foundation)
* **ORM:** Entity Framework Core
* **Bancos de Dados:** SQL Server e SQLite
* **Documentação:** Swagger / OpenAPI

## 📖 Como Executar

1.  Clone o repositório.
2.  Configure a string de conexão do SQL Server no `appsettings.json` da API.
3.  Execute o comando `Update-Database` no Console do Gerenciador de Pacotes para criar as migrações.
4.  Inicie a API, o Simulador e a Interface WPF simultaneamente.

---
*Projeto desenvolvido para fins de monitoramento industrial e persistência distribuída.*
