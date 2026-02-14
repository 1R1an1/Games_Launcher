# Games Launcher

Games Launcher es una aplicación de escritorio desarrollada en WPF (.NET Framework 4.8) que permite gestionar y ejecutar juegos desde una interfaz centralizada, incluyendo configuración individual, descarga de archivos y control de instancia única.

## 🚀 Características
- Lanzamiento de juegos desde una interfaz unificada
- Configuración individual por juego
- Descarga de archivos integrada (FileDownloader)

## 🏗 Arquitectura

El proyecto está estructurado de la siguiente forma:

- **Core/** <br>
  Lógica interna, utilidades, funciones de los juegos y sistema de descarga.
- **Core/FD** <br>
  Logica del sistema de descarga (FileDownloader).
- **Model/** <br>
  Modelos y ViewModels.
- **Views/** <br>
  Vistas principales y componentes visuales.
- **Styles/** <br>
  Estilos y recursos para XAML.
- **Windows/** <br>
  Ventanas principales y de configuración.
- **Infraestructure/** <br>
  Control de instancia única, ventana y notify icon.

## 🧩 Tecnologías usadas

- C#
- WPF (.NET Framework 4.8)
- XAML

## 📌 Estado del proyecto

Proyecto en desarrollo. Puede recibir mejoras de optimización y nuevas funciones.

## 🐞 Reportar problemas
Para errores o sugerencias, abre un *[Issue](https://github.com/1R1an1/Games_Launcher/issues)* en el repositorio indica tu versión de Windows y los pasos para reproducir el problema.

## ⚙ Requisitos

- Windows 10 o superior
- .NET Framework 4.8
- Visual Studio 2022 o superior (recomendado)

## ▶ Cómo ejecutar el proyecto

1. Clonar el repositorio
2. Abrir Games Launcher.sln en Visual Studio
3. Compilar en modo Release o Debug
4. Ejecutar

<!-- COMENTARIO
## 🧠 Objetivo del proyecto

Crear un launcher ligero, personalizable y modular que permita centralizar la gestión de juegos sin depender de plataformas externas.
-->