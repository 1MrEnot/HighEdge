# HighEdge

Solution of the case from Ak Bars Bank at the student hackathon `HighEdge`, held on March 12-13, 2022

### Requirements:
- Secret valut should be distributed among N nodes
- No node should keep entire secret
- The system should provide availability if at least (N * 0.7) of the initial number of nodes are available.
- The vault should have API, that allows create, read and delete secret by its ID


### Short descrtiption:
- Masterless
- Node amout, as well as minimal available node amount are set on startup
- Uses Shamir's Secret Sharing algorithm
- Read, create and delete operations are implemented
- Internode communiacation implemented via gRPC


# HighEdge

Решение кейса от Ак Барс Банка по направлению `системная разработка` на студенческом хакатоне `HighEdge`, проходившего 12-13 марта 2022 года

### Требования:
- Хранилище секретов является распределенным между N устройствами (узлами). 
- Ни на одном узле секрет не должен храниться в полностью. 
- Система должна обеспечивать доступность в случае, если работоспособны хотя бы (N*0.7) от первоначального количества узлов.
- Хранилище должно иметь программный интерфейс, позволяющий записать идентификатор секрета и секрет, получить значение секрета по идентификатору, удалить секрет по идентификатору.

### Краткое описание
- Система masterless
- Кол-во узлов в системе не ограничено и задаётся при старте. Число нод, которые должны быть доступны для восстановления секрета, также задаётся при запуске
- Для разделения и объединения секрета используется аглоритм Шамира (Shamir's Secret Sharing)
- Реализованы операции чтения, добавления, удаления
- Для взаимодействия м/у нодами используется Grpc
