import pika
import json

EXCHANGES = [
    "order.failed.stock",
    "order.success",
    "stock.low"
]

def callback(ch, method, properties, body):
    print("\n📩 EVENTO RECIBIDO")
    print("Exchange:", method.exchange)
    print("RoutingKey:", method.routing_key)
    print("Body:", json.loads(body.decode()))
    print("-----------------------------------")

connection = pika.BlockingConnection(
    pika.ConnectionParameters("localhost")
)
channel = connection.channel()

# Cola temporal que desaparece al cerrar
queue = channel.queue_declare(queue="", exclusive=True).method.queue

# Vincular la cola a cada exchange
for ex in EXCHANGES:
    channel.exchange_declare(exchange=ex, exchange_type="fanout", durable=True)
    channel.queue_bind(exchange=ex, queue=queue)
    print(f"✔️ Escuchando exchange: {ex}")

print("\n🚀 Esperando eventos...\n")

channel.basic_consume(
    queue=queue,
    on_message_callback=callback,
    auto_ack=True
)

channel.start_consuming()
