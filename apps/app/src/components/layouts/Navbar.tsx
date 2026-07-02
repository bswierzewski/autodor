import { UserButton } from "@clerk/react";
import { PackageIcon, UsersThreeIcon } from "@phosphor-icons/react";
import { Link, useRouterState } from "@tanstack/react-router";

import { useMediaQuery } from "@/hooks/use-media-query";
import { cn } from "@/lib/utils";

const navItems = [
	{
		label: "Zamówienia",
		to: "/",
		icon: PackageIcon,
		isActive: (pathname: string) => pathname === "/" || pathname.startsWith("/orders/"),
	},
	{
		label: "Klienci",
		to: "/contractors",
		icon: UsersThreeIcon,
		isActive: (pathname: string) => pathname === "/contractors" || pathname.startsWith("/contractors/"),
	},
] as const;

export function Navbar() {
	const showUserName = useMediaQuery("(min-width: 640px)");
	const pathname = useRouterState({ select: (state) => state.location.pathname });

	return (
		<header className="flex items-center justify-between gap-6 rounded-b-2xl border px-5 py-4 shadow-sm">
			<Link
				className="shrink-0 rounded-sm text-lg font-black outline-none focus-visible:ring-3 focus-visible:ring-ring/50"
				to="/"
			>
				AUTODOR
			</Link>
			<div className="flex items-center gap-4 sm:gap-6">
				<nav aria-label="Główna nawigacja" className="flex items-center gap-2 sm:gap-3">
					{navItems.map(({ label, to, icon: Icon, isActive }) => (
						<Link
							key={to}
							to={to}
							aria-label={label}
							aria-current={isActive(pathname) ? "page" : undefined}
							className={cn(
								"flex items-center gap-2 rounded-full px-3 py-2 text-sm font-medium",
								isActive(pathname) && "text-secondary bg-primary border shadow-sm",
							)}
						>
							<Icon size={18} weight="duotone" />
							<span className="hidden sm:inline">{label}</span>
						</Link>
					))}
				</nav>
				<div aria-hidden="true" className="h-10 w-px border-l" />
				<div className="flex items-center gap-3">
					<UserButton
						appearance={{
							elements: {
								userButtonBox: {
									flexDirection: "row-reverse",
								},
							},
						}}
						showName={showUserName}
					/>
				</div>
			</div>
		</header>
	);
}
