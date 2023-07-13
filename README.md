# Jump Up
Jump Up - небольщая мобильная игра в жанре Hyper Casual, выполненная в архитектуре Composition Root.

Архитектура Composition Root, как и MVVM, осуществляет разделение представления и бизнес-логики. Data Binding реализуется за счет инъекции контекстов типа struct, в основном, состоящих из реактивных свойств и событий (UniRX).

Схема с разделением по логическим слоям
![Image alt](https://github.com/laststare/jumpUp/raw/master/Assets/CompositionTree.png)
