El siguiente juego fue desarrollado en Unity,para crear una interfaz visual agradable y con mejor experiencia de usuario.La mayor parte de la logica del juego recae sobre un script principal(Map.cs).Este es encargado de la generacion del mapa,la logica de movimiento del jugador,entre otras cuestiones.Los otros scripts presentes estan destinados principalmente a cuestiones visuales,a continuacion se hara un breve resumen de la logic del script principal del juego.
La clase Player sera el objeto encargado de representar a cada jugador,contiene variables que describen a cada jugador como el orle,name ,id,etc,tambien contiene variables de tipo gameobject que no son mas que las texturas asociadas a cada jugador.La variable instance ,contiene la instacia de la textura del jugador en pantalla,a traves de ella podremos modificar cuestiones visuales del jugador.La matriz llamada distances,contendra las distancias de la posicion inicial del jugador a todas las otras casillas del tablero,mas adelante se le dara uso.

Generacion del mapa:

El mapa es generado de manera aleatoria totalmente,las dimensiones del mapa deben ser impares ya que los bordes del mapa deben ser paredes.La matriz que contendra la representacion del laberinto comienza inicalmente con paredes en todas sus posiciones.La funcion Generate ,comienza inicicando como camino la posicion 1,1 de la matriz del laberinto.A partir de aqui agrega una tupla a una lista, las cordenadas de  2 celdas consecutivas a esta celda en las 4 direcciones con la siguiente estructura ((x1,y1),(x2,y2)),en caso de que la posicion sea valida.Luego comienza a iterar sobre la lista ,escogiendo un elemento random lo cual contribuye a la ramificacion aleatoria,ese elemento random se elimina de la lista,el elemento sacado,que es una tupla de 2 celdas consecutivas,si la matriz del laberinto en la posicion de la segunda celda de la tupla es una pared,se marcan las 2 celdas de la tupla como camino, y se agrega la los pares de celdas adyacentes a la segunda celda de la tupla para que sean procesadas.Este algoritmo,se dentendra en un punto.Ya nuestro laberinto estaria generado.

Metas de los jugadores:

Cada jugador debe llegar a su correspondiente checkpoint para ganar,con la funcion Add_Checkpoints generamos el checkpoint correspondiente a cada jugador,y para igualar posibilidades se creo una algoritmo que genera los checkpoint de los jugadores a la misma desitancia para cada uno,o con la minima diferencia posible.Se ejecuta un BFS desde la posicion inicial de cada jugador hasta todas las casillas del tablero y se escoge de las distancias de todos los jugadores ,de las mayores distancias,la del jugador que menor distancia posee.A partir de aqui buscamos una casilla para cada jugador correspondiente con la distancia fijada,y si esta casilla se corresponde con el checkpoint de otro jugador o el punto de genracion,se busca otra con la mimsa distancia y en caso de no existir se disminuye la distaancia y se busca de nuevo hasta encontrar una valida.

Movimiento del jugador:

Para obtener las casillas de movimiento validas para un jugador,o sea las casillas accesibles por el jugador,basta con realizar un BFS,que comience desde la posicion actual del jugador,asi obtendremos las distancias desde esa posicion a las demas cedas.Las celdas validas seran aquellas que su distancia es menor o igual que la velocidad del jugador y mayor que 0.

Distribucion aleatoria de trampas y recompensas:

Para esto,se implemento un metodo que dada un numero entre 1 y 100 que llamaremos probablilidad en porcentaje,lo llevaremos a escala de posibilidades por ejemplo un 50 porciento de probabilidad sera una de cada 2 un 20 porciento 1 de cada 5.Basta con dividir 100 por la probabilidad dada ,ya tenemos nuestra numero a escala.Ahora generamos un numero randon entre 1 y la probabilidad llevada a escala si el numero es 1,cayo en un caso positivo.Ahora recorremos cada posicion de la matriz del laberinto en cada posicion generamos un nuemero random,si el caso es positivo ayi colocaremos la trampa o la recompensa.Para hacerlo mas aleatorio aun escogemos una recompensa o trampa aleatoria.

Algunas habilidades:

De las habilidades una de las mas complejas es la de Thor que realiza ataque de area.Funciona de la siguiente manera,se obtiene la psoicion actual del Thor,luego ,dado el rango de alcance R,se recorre la matriz,y el ataqque tendra efecto sobre aquellas casillass que el valor absoluto de la diferencia de la cordenada x de la celda y la de la posicion del jugador sea menor o igual que R,la misma logica hay que seguirla con la cordenada y.Luego ubicamos la posicion de cada jugador en el laberinto y el que se encuentre en alguna de esas casillas son afectados.

Turnos:

La logica de los turnos tamien resulta interesante,como sabemos el player en turno en momento determinado?Muy simple creamos una variable que lleve la cuenta de la cantidad total de turnos,inicializada en 0,asignemos a cada jugador un numero que sera su ID,desde el 0 en adelante,luego TotalTunros%CantidadPlayers me devolvera el ID del jugador en turno actualmente.Al pasar de turno incrementamos la variable que lleva la cuenta de los turnos.En cada turno se estara chequeando constantemente la entrada del jugador,y se chequeara si el jugaor se encuentra encima deuna recompensa ,una tramapa,o si esta encima de su punto de control.La matriz que representa el laberinto es una matriz de strings,que en cada posicion tiene el identificador del elemento que se encuentra alli,una pared,un camino o un checkpoint.Luego existe otra matriz con las mismas dimensiones del laberinto que almacena en cada posicion la trampa o recompensa que alli se encuentra o nada si no hay ninguna de las dos alli.

Mecanica de vida,velocidad,vision y de estados que alteran dichas estadisticas:

Para lograr todo esto es logico que necesitamos un tipo de dato que represente a cada jugador y que desciba su estado actual y caracteristicas,esto lo logramos con la implementacion de la clase Players,de la cual se creara una instancia para cada jugador y sera almacenada en un arreglo.


Manual:

Basta con compilar y ejecutar para comenzar a jugar.Si se ejecuta en la version de Unity recomendada mucho mejor.La jugabilidad es muy intuitiva,ya que el creador se encargo de eso.Seleccione los aspectos de su partidas,los personajes de cada jugador,y comience a jugar.El juego cuenta con un TUTORIAL DETALLADAO Y UNA ENCICLOPEDIA,lealas.


Requisitos:

El siguiente jeugo fue creado en Unity 6000.0.24f1 Beta