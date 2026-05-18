import { UserButton } from "@clerk/react";
import { PackageIcon, UsersThreeIcon } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";

import { useMediaQuery } from "@/hooks/use-media-query";

const navItems = [
	{ label: "Zamówienia", to: "/", icon: PackageIcon, exact: true },
	{ label: "Klienci", to: "/contractors", icon: UsersThreeIcon, exact: false },
] as const;

export function Navbar() {
	const showUserName = useMediaQuery("(min-width: 640px)");

	return (
		<header className="flex items-center justify-between gap-6 rounded-b-2xl border px-5 py-4 shadow-sm">
			<div className="shrink-0">
				<span className="text-lg font-black tracking-[0.2em]">AUTODOR</span>
			</div>
			<div className="flex items-center gap-4 sm:gap-6">
				<nav aria-label="Główna nawigacja" className="flex items-center gap-2 sm:gap-3">
					{navItems.map(({ label, to, icon: Icon, exact }) => (
						<Link
							key={to}
							to={to}
							activeOptions={exact ? { exact: true } : undefined}
							aria-label={label}
							className="flex items-center gap-2 rounded-full px-3 py-2 text-sm font-medium"
							activeProps={{
								className: "text-secondary bg-primary border shadow-sm",
							}}
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
