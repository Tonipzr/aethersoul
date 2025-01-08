# **Aethersoul**

**Breve descripción del proyecto:**
Aethersoul es un videojuego del género Roguelite desarrollado en Unity utilizando Entity Component System (ECS) el framework orientado a datos de Unity.

---

## **Tabla de Contenidos**
1. [Características](#características)
2. [Tecnologías Utilizadas](#tecnologías-utilizadas)
3. [Requisitos del Sistema](#requisitos-del-sistema)
4. [Instalación](#instalación)
5. [Cómo Jugar](#cómo-jugar)
6. [Arquitectura del Proyecto](#arquitectura-del-proyecto)
7. [Capturas de Pantalla](#capturas-de-pantalla)
8. [Créditos](#créditos)
9. [Licencia](#licencia)

---

## **Características**
- **ECS Architecture:** Implementación de Unity ECS para optimizar el rendimiento.

---

## **Tecnologías Utilizadas**
- **Unity:** Versión 2022.3.51f1

---

## **Requisitos del Sistema**
- **Sistema Operativo:** Windows
- **Procesador:** TBD
- **Memoria RAM:** TBD
- **GPU:** TBD
- **Espacio en Disco:** 150MB

---

## **Instalación**
Hay dos formas de ejecutar el juego.

1. Descargando una versión precompilada desde el apartado [Releases](https://github.com/Tonipzr/aethersoul/releases)

2. Ejecutar el juego desde el Editor de Unity. Para ello, se deben seguir los siguientes pasos.
    1. Clonar el repositorio:
    ```bash
        git clone https://github.com/Tonipzr/aethersoul
    ```
    2. Abrir el proyecto descargado mediante Unity, utilizando la versión 2022.3.51f1.
    3. Ejecutar desde el propio editor.

---

## **Cómo Jugar**
Nada más abrir el juego nos aparecerá el menú principal. Desde este menú se puede acceder tanto al juego como a la *Dream City*

### Juego
El juego consiste en matar enemigos para subir de nivel. Cada vez que subimos de nivel obtendremos oro y una elección de mejora.
Estas mejoras se perderán si nuestro personaje muere, pero podremos utilizar el oro en mejoras persistentes que no se perderán.

#### Controles
- El movimiento del personaje funciona mediante **WASD**.
- El **Espacio** permite realizar un *dash*.
- La **E** nos permite interactuar.
- Los botones **1, 2 ,3 y 4** sirven para utilizar las habilidades.
- El botón **Escape** accede al menú de pausa.
- Mediante el botón **P** accedemos al menú de habilidades.

*Nota*: Todos los controles pueden modificarse desde la interfaz del juego.

### DreamCity
La Dream City es el sistema de mejoras persistentes que se ha desarrollado para que no se pierda por completo el progreso de la partida.
Desde este lugar se puede gastar el oro obtenido durante la partida para obtener mejoras permanentes.

---

## **Arquitectura del Proyecto**

El código está dividido siguiendo la estructura ECS.
- Entidades
- Sistemas
- Componentes

---

## **Capturas de Pantalla**

### Menú principal
![Imagen del menú principal](Main_Menu.png)

### Juego
![Imagen del menú principal](Ingame.png)

### Dream City
![Imagen de Dream City](Dream_City.png)
---

## **Créditos**

1. Nature Landscapes (https://free-game-assets.itch.io/nature-landscapes-free-pixel-art) Con licencia disponible en el siguiente en https://craftpix.net/file-licenses/.
2. Enemy Galore I (https:/admurin.itch.io/enemy-galore-1). Con licencia disponible en la página del asset. 
3. Pixel Crystal Animation (https://pekschi.itch.io/crystal-animation). Con licencia disponible en la página del asset. 
4. Pixel Art Top Down Basic (https://cainos.itch.io/pixel-art-top-down-basic). Con licencia disponible en la página del asset. 
5. Effect and Bullet (https://bdragon1727.itch.io/free-effect-and-bullet-16x16). Con licencia disponible en la página del asset. 
6. 750 Effect and FX Pixel All (https://bdragon1727.itch.io/750-effect-and-fx-pixel-all). Con licencia disponible en la página del asset, versión Free. 
7. Hooded Protagonist (https://penzilla.itch.io/hooded-protagonist). Con licencia disponible en la página del asset. 
8. Pixel Art GUI Elements (https://mounirtohami.itch.io/pixel-art-gui-elements). Con licencia disponible en la página del asset.
9. MinimalPixel Font (https://mounirtohami.itch.io/minimalpixel-font). Con licencia disponible en la página del asset.
10. Silver Font (https://poppyworks.itch.io/silver). Con licencia disponible en la página del asset.
11. RPG Essentials SFX (https://leohpaz.itch.io/rpg-essentials-sfx-free). Con licencia disponible en la página del asset. 
12. Fantasy RPG Soundtrack (https://alkakrab.itch.io/fantasy-rpg-soundtrack-music). Con licencia disponible en la página del asset.

---

## **Licencia**

[GNU GPLv3](LICENSE)