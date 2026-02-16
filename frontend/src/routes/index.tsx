import { createFileRoute } from "@tanstack/react-router";
import { Button } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";

export const Route = createFileRoute("/")({
	component: Index,
});

function Index() {
	return (
		<div className="container mx-auto p-8">
			<h1 className="text-4xl font-bold mb-8">Autodor</h1>

			<Card className="max-w-md">
				<CardHeader>
					<CardTitle>Welcome to Autodor!</CardTitle>
					<CardDescription>
						TanStack Router + React Query + shadcn/ui
					</CardDescription>
				</CardHeader>
				<CardContent className="space-y-4">
					<Input placeholder="Enter something..." />
					<Button>Click me</Button>
				</CardContent>
			</Card>
		</div>
	);
}
