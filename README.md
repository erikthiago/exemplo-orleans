# exemplo-orleans
Projeto que visa mostrar os conceitos no orleans e o funcionamento em um projeto mais próximo de uma aplicação real

# Explicação das camadas

## API
 A camada de cliente que vai ser usada para chamar os dados no silo ou diretamente no banco de dados.
 
 ## Grains
 A camada que contém as implemenbtações das interfaces dos grãos.
 
 ## GrainsInterfaces
 A camada que guarda as interfaces que serão implementadas na camada anterior. 
 
 ## Infra
 A camada responsável por fazer as configurações de acesso a dados com o Dapper.
 
 ## Models
 A camada que guarda as entidades de banco de dados representadas em classes.
 
 ## Silo
 A camada responsável por configurar e subir o silo para guardar os grãos da cadamada Grains.
 
 ## Tests
 A camada que agrupra os testes unitários
 
 ## Como rodar o projeto?
 Configure na solução os projetos de inicialização: API e Silo. E dê o play!
