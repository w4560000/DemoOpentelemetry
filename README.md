# Opentelemetry With Jeager In .Net Core
�{���X��g�� https://github.com/niteshsinghal85/blogs/tree/main/DemoOpentelemetry�A�s�W�걵 MSSQL�BRedis�BAPI2 ������

|  �A��			| Port��				|
|  ----			| ----				|
| DemoOpentelemetry.UI	| https://localhost:7108 |
| DemoOpentelemetry.API	| https://localhost:7114 |
| DemoOpentelemetry.API2| https://localhost:7269 |

run jeager
```
docker run --name jaeger -p 13133:13133 -p 16686:16686 -p 4317:4317 -d --restart=unless-stopped jaegertracing/opentelemetry-all-in-one
```