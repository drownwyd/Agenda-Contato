# 📇 Gerenciador de Contatos - WPF

> Aplicação desktop moderna para gerenciamento de contatos desenvolvida em C# com WPF, seguindo o padrão MVVM e arquitetura em camadas.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=c-sharp)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=flat-square&logo=windows)
![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=flat-square&logo=sqlite)

---

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação](#-instalação)
- [Como Usar](#-como-usar)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Banco de Dados](#-banco-de-dados)
- [Validações](#-validações)
- [Capturas de Tela](#-capturas-de-tela)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

---

## 🎯 Sobre o Projeto

O **Gerenciador de Contatos** é uma aplicação desktop completa para Windows que permite gerenciar informações de contatos de forma eficiente e intuitiva. Desenvolvida com as melhores práticas de desenvolvimento .NET, a aplicação oferece uma interface moderna e responsiva.

### Destaques

- ✅ Interface 100% em **Português (Brasil)**
- ✅ Padrão **MVVM** (Model-View-ViewModel)
- ✅ Arquitetura em **camadas** (Data, Repository, Service, ViewModel, View)
- ✅ Persistência com **SQLite** e **Entity Framework Core**
- ✅ Validação robusta de dados
- ✅ Importação/Exportação **CSV**
- ✅ Busca, ordenação e paginação
- ✅ Design moderno com **Material Design**

---

## ✨ Funcionalidades

### Gerenciamento de Contatos

- **➕ Adicionar** novos contatos com validação em tempo real
- **✏️ Editar** informações de contatos existentes
- **🗑️ Excluir** contatos com confirmação
- **👁️ Visualizar** lista completa de contatos

### Recursos Avançados

- **🔍 Busca Inteligente**: Pesquise por nome, sobrenome, empresa, telefone ou email
- **📊 Ordenação**: Ordene por qualquer coluna (nome, empresa, email, etc.)
- **📄 Paginação**: Navegue por grandes listas com controle de itens por página (10, 20, 50, 100)
- **📤 Exportar CSV**: Exporte todos os contatos para arquivo CSV
- **📥 Importar CSV**: Importe contatos em massa de arquivo CSV
- **📸 Foto de Perfil**: Adicione foto aos contatos

### Campos de Contato

- **Nome** (obrigatório)
- **Sobrenome**
- **Empresa**
- **Telefone Principal**
- **Telefone Secundário**
- **Email**
- **Endereço**
- **Observações**
- **Foto**

---

## 🛠️ Tecnologias Utilizadas

### Framework e Linguagem

- **.NET 10.0** - Framework principal
- **C# 12.0** - Linguagem de programação
- **WPF (Windows Presentation Foundation)** - Interface gráfica

### Bibliotecas e Pacotes

- **Entity Framework Core 9.0** - ORM para acesso a dados
- **SQLite** - Banco de dados leve e embutido
- **Microsoft.EntityFrameworkCore.Sqlite** - Provider SQLite para EF Core

### Padrões e Práticas

- **MVVM (Model-View-ViewModel)** - Padrão arquitetural
- **Repository Pattern** - Abstração de acesso a dados
- **Dependency Injection** - Injeção de dependências manual
- **Async/Await** - Programação assíncrona
- **Data Binding** - Vinculação de dados bidirecional
- **INotifyPropertyChanged** - Notificação de mudanças de propriedades

---

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em camadas** bem definida:

```
┌─────────────────────────────────────┐
│         View (XAML)                 │  ← Interface do Usuário
├─────────────────────────────────────┤
│         ViewModel                   │  ← Lógica de Apresentação
├─────────────────────────────────────┤
│         Service                     │  ← Regras de Negócio
├─────────────────────────────────────┤
│         Repository                  │  ← Acesso a Dados
├─────────────────────────────────────┤
│         Data (DbContext)            │  ← Configuração do Banco
├─────────────────────────────────────┤
│         Model                       │  ← Entidades
└─────────────────────────────────────┘
```

### Camadas

1. **Model** (`Models/`)
   - Entidades de domínio
   - Anotações de validação

2. **Data** (`Data/`)
   - `AppDbContext`: Contexto do Entity Framework
   - Configuração do SQLite

3. **Repository** (`Repositories/`)
   - `ContactRepository`: Operações CRUD básicas
   - Abstração do acesso a dados

4. **Service** (`Services/`)
   - `ContactService`: Lógica de negócio e validações
   - `ImportExportService`: Importação/exportação CSV

5. **ViewModel** (`ViewModels/`)
   - `ViewModelBase`: Classe base com INotifyPropertyChanged
   - `RelayCommand`: Implementação de ICommand
   - `MainViewModel`: ViewModel da tela principal
   - `ContactFormViewModel`: ViewModel do formulário

6. **View** (`Views/` e raiz)
   - `MainWindow.xaml`: Tela principal
   - `ContactFormWindow.xaml`: Formulário de contato

7. **Converters** (`Converters/`)
   - `BoolToVisibilityConverter`: Conversão bool → Visibility
   - `StringToVisibilityConverter`: Conversão string → Visibility

---

## 📦 Pré-requisitos

### Requisitos de Sistema

- **Windows 10** ou superior
- **.NET 10.0 SDK** ou superior
- **Visual Studio 2022** (recomendado) ou VS Code

### Instalação do .NET SDK

1. Baixe o .NET SDK: https://dotnet.microsoft.com/download
2. Execute o instalador
3. Verifique a instalação:
   ```bash
   dotnet --version
   ```

---

## 🚀 Instalação

### 1. Clone o Repositório

```bash
git clone https://github.com/seu-usuario/agenda-c#.git](https://github.com/drownwyd/Agenda-Contato.git
cd Agenda-Contato
```

### 2. Restaure as Dependências

```bash
dotnet restore
```

### 3. Compile o Projeto

```bash
dotnet build
```

### 4. Execute a Aplicação

```bash
dotnet run --project src/ContactsApp/ContactsApp.csproj
```

**Ou usando o executável:**

```bash
.\src\ContactsApp\bin\Debug\net10.0-windows\ContactsApp.exe
```

---

## 📖 Como Usar

### Iniciando a Aplicação

1. Execute o arquivo `ContactsApp.exe`
2. A janela principal será aberta mostrando a lista de contatos
3. O banco de dados SQLite será criado automaticamente na primeira execução

### Adicionando um Contato

1. Clique no botão **"➕ Adicionar Contato"**
2. Preencha os campos do formulário:
   - **Nome** é obrigatório
   - Outros campos são opcionais
3. Clique em **"Salvar"**

### Editando um Contato

1. Selecione um contato na lista
2. Clique no botão **"✏️ Editar"**
3. Modifique os campos desejados
4. Clique em **"Salvar"**

### Excluindo um Contato

1. Selecione um contato na lista
2. Clique no botão **"🗑️ Excluir"**
3. Confirme a exclusão

### Buscando Contatos

1. Digite o termo de busca na caixa de pesquisa
2. Clique em **"Buscar"** ou pressione **Enter**
3. A busca procura em: nome, sobrenome, empresa, telefone e email

### Importando Contatos (CSV)

1. Clique em **"📥 Importar CSV"**
2. Selecione o arquivo CSV
3. O formato esperado:
   ```csv
   FirstName,LastName,Company,PrimaryPhone,SecondaryPhone,Email,Address,Notes,PhotoPath
   João,Silva,Empresa X,11987654321,,joao@email.com,"Rua A, 123",,
   ```
4. Contatos duplicados (mesmo telefone) serão ignorados

### Exportando Contatos (CSV)

1. Clique em **"📤 Exportar CSV"**
2. Escolha o local e nome do arquivo
3. Todos os contatos serão exportados

### Paginação

- Use os botões **"◀ Anterior"** e **"Próxima ▶"** para navegar
- Altere o número de itens por página no dropdown
- Veja a página atual e total no rodapé

---

## 📁 Estrutura do Projeto

```
agenda-c#/
├── src/
│   ├── ContactsApp/                    # Projeto principal
│   │   ├── Converters/                 # Conversores XAML
│   │   │   ├── BoolToVisibilityConverter.cs
│   │   │   └── StringToVisibilityConverter.cs
│   │   ├── Data/                       # Camada de dados
│   │   │   └── AppDbContext.cs
│   │   ├── Models/                     # Modelos de domínio
│   │   │   └── Contact.cs
│   │   ├── Repositories/               # Repositórios
│   │   │   └── ContactRepository.cs
│   │   ├── Resources/                  # Recursos (imagens, etc)
│   │   ├── Services/                   # Serviços de negócio
│   │   │   ├── ContactService.cs
│   │   │   └── ImportExportService.cs
│   │   ├── ViewModels/                 # ViewModels MVVM
│   │   │   ├── ContactFormViewModel.cs
│   │   │   ├── MainViewModel.cs
│   │   │   ├── RelayCommand.cs
│   │   │   └── ViewModelBase.cs
│   │   ├── Views/                      # Views adicionais
│   │   │   ├── ContactFormWindow.xaml
│   │   │   └── ContactFormWindow.xaml.cs
│   │   ├── App.xaml                    # Configuração do app
│   │   ├── App.xaml.cs
│   │   ├── MainWindow.xaml             # Janela principal
│   │   ├── MainWindow.xaml.cs
│   │   └── ContactsApp.csproj          # Arquivo de projeto
│   └── ContactsApp.Tests/              # Projeto de testes
├── ContactsApp.slnx                    # Solução
└── README.md                           # Este arquivo
```

---

## 💾 Banco de Dados

### SQLite

A aplicação usa **SQLite** como banco de dados, que é:
- ✅ **Leve**: Não requer instalação de servidor
- ✅ **Portátil**: Arquivo único e fácil de mover
- ✅ **Rápido**: Ótimo desempenho para aplicações desktop
- ✅ **Confiável**: ACID compliant

### Localização do Banco

O arquivo `contacts.db` é criado automaticamente em:
```
src/ContactsApp/bin/Debug/net10.0-windows/contacts.db
```

### Estrutura da Tabela Contacts

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | INTEGER | Chave primária (auto-incremento) |
| `FirstName` | TEXT | Nome (obrigatório, 2-100 caracteres) |
| `LastName` | TEXT | Sobrenome |
| `Company` | TEXT | Empresa |
| `PrimaryPhone` | TEXT | Telefone principal |
| `SecondaryPhone` | TEXT | Telefone secundário |
| `Email` | TEXT | Email (validado) |
| `Address` | TEXT | Endereço completo |
| `Notes` | TEXT | Observações |
| `PhotoPath` | TEXT | Caminho da foto |
| `CreatedAt` | DATETIME | Data de criação |
| `UpdatedAt` | DATETIME | Data de atualização |

### Migrations

O banco é criado automaticamente usando:
```csharp
dbContext.Database.EnsureCreated();
```

---

## ✅ Validações

### Validações Implementadas

#### Nome
- ✅ **Obrigatório**
- ✅ Mínimo 2 caracteres
- ✅ Máximo 100 caracteres

#### Email
- ✅ Formato válido (usuario@dominio.com)
- ✅ Validação usando `MailAddress`

#### Telefone
- ✅ Mínimo 10 dígitos
- ✅ Máximo 15 dígitos
- ✅ Aceita formatação: `(11) 98765-4321`, `+55 11 98765-4321`
- ✅ **Não permite duplicatas** (mesmo telefone em contatos diferentes)

### Mensagens de Erro (PT-BR)

- "Nome é obrigatório"
- "Formato de email inválido"
- "Formato de telefone principal inválido"
- "Formato de telefone secundário inválido"
- "Número de telefone já existe para o contato: [Nome]"

---

## 🎨 Capturas de Tela

### Tela Principal
- Lista de contatos com DataGrid
- Barra de busca
- Botões de ação (Adicionar, Editar, Excluir, Importar, Exportar)
- Paginação
- Barra de status

### Formulário de Contato
- Campos organizados verticalmente
- Validação em tempo real
- Exibição de erros
- Botões Salvar/Cancelar
- Seleção de foto com preview

---

## 🧪 Testes

### Executar Testes

```bash
dotnet test
```

### Estrutura de Testes

O projeto `ContactsApp.Tests` contém:
- Testes unitários para Services
- Testes unitários para ViewModels
- Testes de validação

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Siga os passos:

1. **Fork** o projeto
2. Crie uma **branch** para sua feature:
   ```bash
   git checkout -b feature/MinhaFeature
   ```
3. **Commit** suas mudanças:
   ```bash
   git commit -m 'Adiciona MinhaFeature'
   ```
4. **Push** para a branch:
   ```bash
   git push origin feature/MinhaFeature
   ```
5. Abra um **Pull Request**

### Padrões de Código

- Use **C# Conventions**
- Siga o padrão **MVVM**
- Adicione **comentários** em código complexo
- Escreva **testes** para novas funcionalidades
- Mantenha mensagens em **Português (Brasil)**

---

## 📝 Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 👨‍💻 Autor

Desenvolvido por Drownwyd com ❤️ usando C# e WPF

---

## 📞 Suporte

Encontrou um bug? Tem uma sugestão?

- Abra uma [Issue](https://github.com/seu-usuario/agenda-c#/issues)
- Entre em contato: seu-email@exemplo.com

---

## 🔄 Changelog

### Versão 1.0.0 (2025-11-20)

#### ✨ Funcionalidades
- ✅ CRUD completo de contatos
- ✅ Busca por múltiplos campos
- ✅ Ordenação e paginação
- ✅ Importação/Exportação CSV
- ✅ Validação robusta de dados
- ✅ Interface 100% em português
- ✅ Banco de dados SQLite

#### 🎨 Interface
- ✅ Design moderno com Material Design
- ✅ Cores vibrantes e responsivas
- ✅ Feedback visual (hover, loading)
- ✅ Mensagens de erro claras

#### 🏗️ Arquitetura
- ✅ Padrão MVVM completo
- ✅ Arquitetura em camadas
- ✅ Separação de responsabilidades
- ✅ Código limpo e manutenível

---

## 🚀 Roadmap

### Próximas Funcionalidades

- [ ] Backup automático do banco de dados
- [ ] Exportação para Excel
- [ ] Importação de vCard
- [ ] Grupos/Categorias de contatos
- [ ] Histórico de alterações
- [ ] Busca avançada com filtros
- [ ] Temas claro/escuro
- [ ] Sincronização com nuvem
- [ ] Aplicativo mobile (Xamarin/MAUI)

---

## 📚 Recursos Adicionais

### Documentação

- [Documentação do .NET](https://docs.microsoft.com/dotnet/)
- [WPF Tutorial](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [MVVM Pattern](https://docs.microsoft.com/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)

### Ferramentas Recomendadas

- **Visual Studio 2022** - IDE principal
- **SQLite Browser** - Visualizar banco de dados
- **Git** - Controle de versão
- **Postman** - Testar APIs (futuro)

---

**⭐ Se este projeto foi útil, considere dar uma estrela no GitHub!**
