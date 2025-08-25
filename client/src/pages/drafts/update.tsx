import toast from "react-hot-toast";
import {
  type DraftDetail,
  type DraftFormData,
} from "../../models/generated-client";
import {
  useLoaderData,
  useNavigate,
  type LoaderFunctionArgs,
} from "react-router-dom";
import { useForm, type SubmitHandler } from "react-hook-form";
import { Loading } from "../../components/base/loading";
import { draftClient } from "../../api-clients";

export async function draftLoader({ params }: LoaderFunctionArgs) {
  const response = draftClient.get(Number.parseInt(params.id!));
  return response;
}

export default function DraftUpdate() {
  const draft = useLoaderData() as DraftDetail;
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DraftFormData>();
  const navigate = useNavigate();

  if (!draft) return <Loading />;

  const onSubmit: SubmitHandler<DraftFormData> = async (data) => {
    const promise = draftClient.update(draft.id!, data);
    await toast.promise(promise, {
      success: "Draft updated successfully",
      error: "Draft update failed",
      loading: "Updating draft...",
    });
    navigate("/draft");
  };

  return (
    <div className="card w-full bg-neutral shadow-xl">
      <div className="card-body">
        <h2 className="card-title">Update post</h2>
        <form method="post" onSubmit={handleSubmit(onSubmit)}>
          <div className="form-control">
            <input
              placeholder="Title"
              type="text"
              defaultValue={draft.title!}
              className={`input input-bordered w-full max-w-xs ${
                errors.title && "input-error"
              }`}
              {...register("title", { required: true })}
            />
            <small className="text-error">{errors.title?.message}</small>
          </div>
          <div className="form-control">
            <textarea
              placeholder="Content"
              defaultValue={draft.content!}
              className={`textarea textarea-bordered w-full ${
                errors.content && "input-error"
              }`}
              {...register("content", { required: true })}
            />
            <small className="text-error">{errors.content?.message}</small>
          </div>
          <div className="form-control">
            <label className="label cursor-pointer">
              <span className="label-text">Publish</span>
              <input
                type="checkbox"
                className="toggle"
                {...register("publish", { value: false })}
              />
            </label>
            <small className="text-error">{errors.publish?.message}</small>
          </div>
          <div className="card-actions justify-end">
            <button type="submit" className="btn btn-primary">
              Save
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
