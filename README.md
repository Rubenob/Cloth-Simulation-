# Cloth-Simulation
[![Gameplay Video](Video)](https://youtu.be/GTATqVjjhpI)

En la escena se encuentra un plano, el cual tiene el script “MassSpring” y un cubo que tiene
asociado el script “Fixer”. En el script del cubo podemos arrastrar el objeto que queremos que
fije los vértices, en este caso sería el plano que funciona como tela. El cual comprueba que
nodos están dentro de él y los fija.
Para ejecutar la simulación solo se deben ajustar los parámetros que encontramos en el script
asociado al plano, donde podemos elegir el time step, la masa, el sifftnes de los muelles tanto
de tracción como de flexión y el damping o amortiguamiento.
Para el primer requisito lo que se ha realizado han sido dos scripts, Node y Spring, los cuales no
son monobehaviour y así poder coger la malla de cualquier objeto y aplicándole el script de
MassSpring se cree un nodo por cada vértice de la malla y un muelle que los una de dos en
dos. Por último, calculamos las físicas en los nodos y los muelles para conseguir sus nuevas
posiciones y luego, pasarles esas nuevas posiciones a los vértices de la malla y así conseguir
una tela con físicas realistas.
Para el segundo requisito se ha creado el script “Fixer”, al cual se le pasa el objeto del que
queremos fijar los vértices. Una vez pasado se comprueba que nodos de la malla del objecto
que se quiere fijar están dentro del collider de la caja para establecer su propiedad de fijos a
true. Esto se realiza con el método contains de los límites de la caja. Por último, para que la
tela se mueva con la caja se calcula la cantidad de movimiento que se ha realizado y esta se
suma a la posición de los nodos fijos.
Para el tercer requisito se ha realizado el script “Edge”, donde se guardan las aristas y se
concreta si son de tracción o flexión antes de convertirlas a muelles. Para poder comparar se
hace que nuestra clase Edge herede de “IComparable”, donde debemos declarar un método
que indique como debe comparar las aristas. Una vez tenemos todas las aristas, se ordenan y
se van creando los muelles a la vez que se comparan con el anterior que se ha creado, si es
igual no se vuelve a crear si no que se crea un muelle de flexión entra los dos vértices
opuestos.
Con respecto a las funcionalidades adicionales se han implementado 3:
- Amortiguamiento: se crea una variable damping, con la que se controla la cantidad de
amortiguamiento que tendrán los nodos y muelles. Esta variable se pasa tanto a
vértices como a muelles para poder calcualar la fuerza de amortiguamiento absoluto
sobre los vértices y la de amortiguamiento de deformación en los muelles y añadirlas
al resto de fuerzas respectivamente.
- Aspectos visuales y/o de interacción: se ha añadido un shader a la tela para que sea
visible por las dos partes. Se crea un material al que añadimos el shader y se le arrastra
al plano. También se ha añadido la posibilidad de mover el fijador y que la tela se
mueva con él, como ya se comento en el segundo requisito.
- Fuerza de viento: para este requisito se ha creado el script “Triangle” y se ha añadido
las variables de velocidad de viento, fricción de viento y si está o no el viento activado
a MassSpring para poder controlar el comportamiento del viento. Por cada triangulo
que tenemos en la escena cogemos sus vértices y creamos un objeto de esta clase, en
el cual se calcula la fuerza del viento y se aplica dividida por tres a cada uno de ellos. 
