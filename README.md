# Opentelemetry With Jeager In .Net Core
程式碼改寫至 https://github.com/niteshsinghal85/blogs/tree/main/DemoOpentelemetry，新增串接 MSSQL、Redis、API2 做測試

|  服務			| Port號				|
|  ----			| ----				|
| DemoOpentelemetry.UI	| https://localhost:7108 |
| DemoOpentelemetry.API	| https://localhost:7114 |
| DemoOpentelemetry.API2| https://localhost:7269 |

run jeager
```
docker run --name jaeger -p 13133:13133 -p 16686:16686 -p 4317:4317 -d --restart=unless-stopped jaegertracing/opentelemetry-all-in-one
```