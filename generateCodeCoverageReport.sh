#!/bin/bash
####################################################################################
# Simple script to generate html-reports for code coverage.
#
# Prerequisite: dotnet-reportgenerator-globaltool
#
# If reportgenerator is not installed, install it using:
#   dotnet tool install dotnet-reportgenerator-globaltool --version 4.0.5 --global
####################################################################################
# Clean-up
rm -rf output/reports
# Generate actual report
reportgenerator "-reports:./output/coverage.cobertura.xml" "-targetdir:output/reports" "-reporttypes:HTMLInline;HTMLChart"