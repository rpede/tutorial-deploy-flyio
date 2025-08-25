import { AuthClient, BlogClient, DraftClient } from "./models/generated-client";

export const authClient = new AuthClient();
export const blogClient = new BlogClient();
export const draftClient = new DraftClient();
