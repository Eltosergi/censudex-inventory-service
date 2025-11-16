import pika
import json
import uuid 

# Configuración de RabbitMQ
RABBITMQ_HOST = "localhost"
USER = "guest"
PASS = "guest"

credentials = pika.PlainCredentials(USER, PASS)
connection = pika.BlockingConnection(
    pika.ConnectionParameters(host=RABBITMQ_HOST, credentials=credentials)
)
channel = connection.channel()

# Exchange que MassTransit usa para OrderCreatedEvent
exchange_name = "censudex_inventory_service.src.DTOs:OrderCreatedEvent"

# Asegurar que el exchange existe (MassTransit lo crea automáticamente, pero por si acaso)
channel.exchange_declare(exchange=exchange_name, exchange_type='fanout', durable=True)

# Crear mensaje
message = {
    "Items": [
        {
            "ProductId": "ca2da015-464e-4186-b152-b1730b7258d3", # Ejemplo de ProductId Cambier por uno válido
            "Quantity": 3 # Cantidad del producto cambiar según necesidad
        }
    ]
}

body = json.dumps(message)

# Publicar mensaje
channel.basic_publish(
    exchange=exchange_name,
    routing_key="",   # Para fanout no se usa routing key
    body=body
)

print("📤 Mensaje OrderCreatedEvent enviado:")
print(body)

connection.close()
