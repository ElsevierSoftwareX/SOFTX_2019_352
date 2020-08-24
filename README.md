[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.3541136.svg)](https://doi.org/10.5281/zenodo.3541136)

# GeoReVi

<img align="right" src="docs/Icon.png" width="50px">
GeoReVi (Geological Reservoir Virtualization) is an open-source data management and analysis tool for subsurface characterization. Data is stored in a local or server database dependent on the specific needs. The software runs on all Windows systems.

## License

This project is licensed under the GNU General Public License v3.0 License - see the [LICENSE.md](LICENSE.md) file for details

The *local* version of the software is freely available.

## Requirements
OS: Microsoft Windows

## Built With
* [Windows Presentation Foundation](https://docs.microsoft.com/de-de/dotnet/framework/wpf/) - Main application framework
* [Entity framework](https://docs.microsoft.com/de-de/ef/) - The database framework used
* [HelixToolkit](https://github.com/helix-toolkit/helix-toolkit}{HelixToolkit.WPF) - 3D visualization
* [Managed Extensibility Framework](https://docs.microsoft.com/de-de/dotnet/framework/mef/) - Modular development
* [Accord.NET](http://accord-framework.net) - Linear Algebra and Machine Learning helper
* [Caliburn.Micro](https://caliburnmicro.com/) - Awesome MVVM development framework
* [FontAwesomeWPF](https://github.com/charri/Font-Awesome-WPF/) - Icon provider
* [LiteDB](https://www.litedb.org/) - Embedded NoSQL database
* [GeoAPI](https://github.com/NetTopologySuite/GeoAPI) - Coordinate conversion and projection framework
* [MoreLinq](https://morelinq.github.io) - Helpful Query Framework
* [Extended WPF Toolkit™](https://github.com/xceedsoftware/wpftoolkit) - Providing nice UI controls like the AvalonDock
* [Math.NET](https://www.mathdotnet.com/) - Math magic
* [Alglib](https://www.alglib.net/) - Solving sparse systems of linear equations


## Getting started
To get started with GeoReVi you have to download the executable file [GeoReViv1.0.1.exe](https://github.com/ApirsAL/GeoReVi/blob/master/binaries/GeoReViv1.0.1.exe) in the **binaries** folder and run the installation. Tutorials can be found in the user manual. Exemplary datasets and samples are provided in the **docs** folders and in the zip archive. To setup a multi-user-environment you have to change the section including the connection string in your GeoReVi.exe.config file in the installation folder to the according database. However, then a properly structured database with many stored procedures is needed. Contact us under contact@georevi.com for more information.

### Creating facies models
<img src="docs/Samples/Interpolations/FaciesModel.png" width="60%">


## The Scope
* Multi-tiered, extendable software application that aims to support geoscientists in common workflows during subsurface characterization
* Geoscientific data management and analysis including petrophysical, geochemical, hydraulic, granulometric, magnetic, mechanical, sedimentary, elastic and radiogenic properties eased by a graphical user interface
* Providing a rich selection of data mining algorithms and visualization techniques specifically aimed to characterize subsurface regions

<img src="docs/Samples/Interpolations/CubeMeasurements.gif" width="60%">

## Types of charts
* scatter charts
* bubble charts
* bar charts
* box-whisker-charts
* semivariogram charts
* ternary charts
* 3D charts

### Three-dimensional outcrop model
<img src="docs/Samples/Meshes/OutcropModeling.png" width="60%">

### Three-dimensional property model of the aluminum-oxide content of a rock cube
<div class="sketchfab-embed-wrapper">
    <iframe title="A 3D model" width="640" height="480" src="https://sketchfab.com/models/1a256da34de445ab8c28d7fe38415ffd/embed?autostart=1&amp;ui_controls=1&amp;ui_infos=1&amp;ui_inspector=1&amp;ui_stop=1&amp;ui_watermark=1&amp;ui_watermark_link=1" frameborder="0" allow="autoplay; fullscreen; vr" mozallowfullscreen="true" webkitallowfullscreen="true"></iframe>
    <p style="font-size: 13px; font-weight: normal; margin: 5px; color: #4A4A4A;">
        <a href="https://sketchfab.com/3d-models/aluminum-oxide-composition-of-a-small-rock-cube-1a256da34de445ab8c28d7fe38415ffd?utm_medium=embed&utm_source=website&utm_campaign=share-popup" target="_blank" style="font-weight: bold; color: #1CAAD9;">Aluminum-oxide composition of a small rock cube</a>
        by <a href="https://sketchfab.com/GeoReVi?utm_medium=embed&utm_source=website&utm_campaign=share-popup" target="_blank" style="font-weight: bold; color: #1CAAD9;">GeoReVi</a>
        on <a href="https://sketchfab.com?utm_medium=embed&utm_source=website&utm_campaign=share-popup" target="_blank" style="font-weight: bold; color: #1CAAD9;">Sketchfab</a>
    </p>
</div>

### Sequential simulation of the porosity
<img src="docs/Samples/Meshes/SGS.gif" width="60%" style="align-self:center;">

## Algorithms
* Semivariogram
* Indicator variogram
* p-value Inverse Distance Weighting
* Simple Kriging
* Ordinary Kriging
* Universal Kriging
* Direct Sequential Simulation
* Leave p-out cross-validation
* Correlation (Spearman and Pearson)
* Regression (linear and curvilinear)
* Principal Component Analysis
* Cluster Analysis
* Self-organizing Maps

### Exploring the uncertainty space
<img src="docs/Samples/Meshes/SGS_Uncertainty_Space.gif" width="60%">

### Mapping geological features in 3-D
<img src="docs/Samples/Mapping/MappingPointsExtract.gif" width="60%">

## Contributing
Individual view-models and views can be developed and integrated into GeoReVi.

If bugs or any other issues are identified we would be grateful for reporting those in the [Issues](https://github.com/ApirsAL/GeoReVi/issues) or via [email](contact@georevi.com).

## Authors

* **Adrian Linsel** - *Initial work* - [ApirsAL](https://github.com/ApirsAL) - contact@georevi.com

## Acknowledgments

* Thanks go to all the people who contributed to the test data sets of GeoReVi!

### Reservoir-scale temperature distribution
<img src="docs/Samples/FrontCover/GitHubFrontCover.png" width="60%">

