# Descripción e Informe

El siguiente juego fue desarrollado en Unity, para crear una interfaz visual agradable y con mejor experiencia de usuario. La mayor parte de la lógica del juego recae sobre un script principal(Map.cs). Este es encargado de la generación del mapa, la lógica de movimiento del jugador, entre otras cuestiones. Los otros scripts presentes están destinados principalmente a cuestiones visuales, a continuación se hará un breve resumen de la lógica del script principal del juego.
La clase Player será el objeto encargado de representar a cada jugador, contiene variables que describen a cada jugador como el role,name ,id,etc,también contiene variables de tipo gameobject que no son más que las texturas asociadas a cada jugador. La variable instance ,contiene la instacia de la textura del jugador en pantalla,a través de ella podremos modificar cuestiones visuales del jugador.La matriz llamada distances, contendrá las distancias de la posición inicial del jugador a todas las otras casillas del tablero,más adelante se le dará uso.

## Generacion del mapa:

El mapa es generado de manera aleatoria totalmente,las dimensiones del mapa deben ser impares ya que los bordes del mapa deben ser paredes.La matriz que contendrá la representación del laberinto comienza inicalmente con paredes en todas sus posiciones.La función Generate ,comienza iniciando como camino la posición 1,1 de la matriz del laberinto.A partir de aquí agrega una tupla a una lista, las cordenadas de  2 celdas consecutivas a esta celda en las 4 direcciones con la siguiente estructura ((x1,y1),(x2,y2)),en caso de que la posición sea valida.Luego comienza a iterar sobre la lista ,escogiendo un elemento random lo cual contribuye a la ramificación aleatoria,ese elemento random se elimina de la lista,el elemento sacado,que es una tupla de 2 celdas consecutivas,si la matriz del laberinto en la posicion de la segunda celda de la tupla es una pared,se marcan las 2 celdas de la tupla como camino, y se agrega la los pares de celdas adyacentes a la segunda celda de la tupla para que sean procesadas.Este algoritmo,se detendrá en un punto.Ya nuestro laberinto estaría generado.

## Metas de los jugadores:

Cada jugador debe llegar a su correspondiente checkpoint para ganar,con la función Add_Checkpoints generamos el checkpoint correspondiente a cada jugador,y para igualar posibilidades se creo una algoritmo que genera los checkpoint de los jugadores a la misma desitancia para cada uno,o con la minima diferencia posible.Se ejecuta un BFS desde la posición inicial de cada jugador hasta todas las casillas del tablero y se escoge de las distancias de todos los jugadores ,de las mayores distancias,la del jugador que menor distancia posee.A partir de aquí buscamos una casilla para cada jugador correspondiente con la distancia fijada,y si esta casilla se corresponde con el checkpoint de otro jugador o el punto de generación,se busca otra con la misma distancia y en caso de no existir se disminuye la distancia y se busca de nuevo hasta encontrar una válida.

## Movimiento del jugador:

Para obtener las casillas de movimiento válidas para un jugador,o sea las casillas accesibles por el jugador, basta con realizar un BFS,que comience desde la posición actual del jugador,asi obtendremos las distancias desde esa posición a las demás cedlas.Las celdas válidas serán aquellas que su distancia es menor o igual que la velocidad del jugador y mayor que 0.

## Distribución aleatoria de trampas y recompensas:

Para esto,se implementó un método que dada un número entre 1 y 100 que llamaremos probabilidad en porcentaje,lo llevaremos a escala de posibilidades por ejemplo un 50 porciento de probabilidad será una de cada 2 un 20 porciento 1 de cada 5. Basta con dividir 100 por la probabilidad dada ,ya tenemos nuestra número a escala.Ahora generamos un número random entre 1 y la probabilidad llevada a escala si el número es 1,cayó en un caso positivo. Ahora recorremos cada posición de la matriz del laberinto en cada posición generamos un número random, si el caso es positivo allí colocaremos la trampa o la recompensa.Para hacerlo más aleatorio aun escogemos una recompensa o trampa aleatoria.

##  Algunas habilidades:

De las habilidades una de las más complejas es la de Thor que realiza ataque de area.Funciona de la siguiente manera,se obtiene la posición actual de Thor, luego , dado el rango de alcance R,se recorre la matriz,y el ataque tendra efecto sobre aquellas casillas que el valor absoluto de la diferencia de la cordenada x de la celda y la de la posicion del jugador sea menor o igual que R,la misma logica hay que seguirla con la cordenada y. Luego ubicamos la posición de cada jugador en el laberinto y el que se encuentre en alguna de esas casillas son afectados.

## Turnos:

La lógica de los turnos también resulta interesante, como sabemos el player en turno en momento determinado?Muy simple creamos una variable que lleve la cuenta de la cantidad total de turnos, inicializada en 0,asignemos a cada jugador un número que será su ID,desde el 0 en adelante,luego TotalTurnos%CantidadPlayers me devolverá el ID del jugador en turno actualmente.Al pasar de turno incrementamos la variable que lleva la cuenta de los turnos.En cada turno se estará chequeando constantemente la entrada del jugador,y se chequeara si el jugador se encuentra encima de una recompensa ,una trampa,o si esta encima de su punto de control.La matriz que representa el laberinto es una matriz de strings,que en cada posición tiene el identificador del elemento que se encuentra allí,una pared,un camino o un checkpoint.Luego existe otra matriz con las mismas dimensiones del laberinto que almacena en cada posición la trampa o recompensa que allí se encuentra o nada si no hay ninguna de las dos allí.

## Mecánica de vida,velocidad,visión y de estados que alteran dichas estadísticas:

Para lograr todo esto es lógico que necesitamos un tipo de dato que represente a cada jugador y que describa su estado actual y características,esto lo logramos con la implementación de la clase Players,de la cual se creara una instancia para cada jugador y será almacenada en un arreglo.Las características específicas de cada personaje,fueron almacenadas en diccionarios,que hacen la función de bases de datos


## Manual:

Basta con compilar y ejecutar para comenzar a jugar.Si se ejecuta en la versión de Unity recomendada mucho mejor.La jugabilidad es muy intuitiva,ya que el creador se encargo de eso. Seleccione los aspectos de su partidas,los personajes de cada jugador,y comience a jugar.El juego cuenta con un TUTORIAL DETALLADAO Y UNA ENCICLOPEDIA, lealas.


# Requisitos:

`El siguiente juego fue creado en Unity 6000.0.24f1 Beta`

# Contacto
## Telegram 
[Necesita contactarme?](https://t.me/cuban_developer)